using Ibatech.Domain.DTOs;
using Ibatech.Domain.Entities;
using Ibatech.Domain.Enums;
using Ibatech.Domain.Interfaces.Repositories;
using Ibatech.Domain.Interfaces.Services;
using Ibatech.Repository.UnitOfWork;
using Ibatech.Services.Importacao;
using Ibatech.Services.Mappers;
using Microsoft.Extensions.Logging;

namespace Ibatech.Services.Implementations;

public sealed class EntradaCompraImportacaoService(
    IProdutoRepository produtoRepository,
    IEstoqueRepository estoqueRepository,
    IEntradaCompraRepository entradaRepository,
    IFornecedorRepository fornecedorRepository,
    IUsuarioRepository usuarioRepository,
    IUnitOfWork uow,
    ILogger<EntradaCompraImportacaoService> logger)
    : IEntradaCompraImportacaoService
{
    public async Task<EntradaCompraImportacaoResultadoDto> ImportarAsync(
        Stream arquivo,
        string nomeArquivo,
        long tamanhoArquivo,
        Guid fornecedorId,
        string numeroDocumento,
        DateTime dataEntrada,
        decimal valorFrete,
        decimal valorDesconto,
        decimal outrasDespesas,
        string? observacao,
        Guid usuarioId,
        CancellationToken ct = default)
    {
        var sanitizedFileName = Path.GetFileName(nomeArquivo);

        if (usuarioId == Guid.Empty)
            throw new ArgumentException("Usuário inválido.");

        if (fornecedorId == Guid.Empty)
            throw new ArgumentException("Fornecedor inválido.");

        if (string.IsNullOrWhiteSpace(numeroDocumento))
            throw new ArgumentException(
                "Número do documento é obrigatório.");

        var usuario =
            await usuarioRepository.ObterPorIdAsync(
                usuarioId,
                ct);

        if (usuario is null || !usuario.Ativo)
            throw new InvalidOperationException(
                "Usuário responsável não encontrado ou inativo.");

        if (usuario.Role != RoleUsuario.Admin &&
            usuario.Role != RoleUsuario.Estoque)
        {
            throw new InvalidOperationException(
                "Usuário sem permissão para importar entradas de compra.");
        }

        var fornecedor =
            await fornecedorRepository.ObterPorIdAsync(
                fornecedorId,
                ct);

        if (fornecedor is null || !fornecedor.Ativo)
            throw new InvalidOperationException(
                "Fornecedor não encontrado ou inativo.");

        if (await entradaRepository.ExisteDocumentoAsync(
                fornecedorId,
                numeroDocumento,
                ct))
        {
            throw new InvalidOperationException(
                "Já existe uma entrada com este documento para o fornecedor informado.");
        }

        logger.LogInformation(
            "Iniciando importação de entrada de compra. " +
            "Arquivo: {FileName}. Documento: {Documento}.",
            sanitizedFileName,
            numeroDocumento);

        var reader =
            new ProdutoImportacaoPlanilhaReader();

        var leitura =
            await reader.LerAsync(
                arquivo,
                nomeArquivo,
                tamanhoArquivo,
                ct);

        if (!leitura.Sucesso)
        {
            return CriarResultadoErro(
                sanitizedFileName,
                leitura.TotalLinhas,
                leitura.Erros);
        }

        var validador =
            new ProdutoImportacaoLinhaValidador();

        var validacao =
            validador.Validar(
                leitura.Linhas!);

        if (!validacao.Sucesso)
        {
            return CriarResultadoErro(
                sanitizedFileName,
                leitura.TotalLinhas,
                validacao.Erros);
        }

        var linhas =
            validacao.Linhas;

        var erros =
            new List<ProdutoImportacaoErroDto>();

        // Compra real não pode possuir item com quantidade zero.
        foreach (var linha in linhas)
        {
            if (linha.QuantidadeInicial <= 0)
            {
                erros.Add(
                    new ProdutoImportacaoErroDto
                    {
                        Linha = linha.NumeroLinha,
                        Campo = "QuantidadeInicial",
                        Valor =
                            linha.QuantidadeInicial.ToString(),
                        Mensagem =
                            "Na importação de uma entrada de compra a quantidade deve ser maior que zero."
                    });
            }
        }

        // SKU obrigatório para a carga de compra.
        foreach (var linha in linhas)
        {
            if (string.IsNullOrWhiteSpace(
                    linha.CodigoSku))
            {
                erros.Add(
                    new ProdutoImportacaoErroDto
                    {
                        Linha = linha.NumeroLinha,
                        Campo = "CodigoSku",
                        Valor = linha.CodigoSku,
                        Mensagem =
                            "O SKU é obrigatório na importação de entrada de compra."
                    });
            }
        }

        // SKUs repetidos dentro da própria planilha.
        var skusDuplicados =
            linhas
                .Where(x =>
                    !string.IsNullOrWhiteSpace(
                        x.CodigoSku))
                .GroupBy(
                    x => x.CodigoSku!.Trim(),
                    StringComparer.OrdinalIgnoreCase)
                .Where(x => x.Count() > 1);

        foreach (var grupo in skusDuplicados)
        {
            foreach (var linha in grupo)
            {
                erros.Add(
                    new ProdutoImportacaoErroDto
                    {
                        Linha = linha.NumeroLinha,
                        Campo = "CodigoSku",
                        Valor = linha.CodigoSku,
                        Mensagem =
                            "O SKU está duplicado na própria planilha."
                    });
            }
        }

        if (erros.Count > 0)
        {
            return CriarResultadoErro(
                sanitizedFileName,
                leitura.TotalLinhas,
                erros);
        }

        // Nenhum SKU da planilha pode existir previamente.
        // Esta primeira versão do importador é para criação do catálogo real.
        var skus =
            linhas
                .Select(x => x.CodigoSku!.Trim())
                .Distinct(
                    StringComparer.OrdinalIgnoreCase)
                .ToList();

        var skusExistentes =
            await produtoRepository
                .ObterSkusExistentesAsync(
                    skus,
                    ct);

        foreach (var linha in linhas)
        {
            if (skusExistentes.Contains(
                    linha.CodigoSku!.Trim(),
                    StringComparer.OrdinalIgnoreCase))
            {
                erros.Add(
                    new ProdutoImportacaoErroDto
                    {
                        Linha = linha.NumeroLinha,
                        Campo = "CodigoSku",
                        Valor = linha.CodigoSku,
                        Mensagem =
                            "Já existe um produto cadastrado com este SKU."
                    });
            }
        }

        if (erros.Count > 0)
        {
            return CriarResultadoErro(
                sanitizedFileName,
                leitura.TotalLinhas,
                erros);
        }

        var entrada =
            new EntradaCompra(
                fornecedorId,
                numeroDocumento,
                dataEntrada,
                valorFrete,
                valorDesconto,
                outrasDespesas,
                usuarioId,
                observacao);

        var produtos =
            new List<Produto>(
                linhas.Count);

        var estoques =
            new List<Estoque>(
                linhas.Count);

        foreach (var linha in linhas)
        {
            /*
             * ATENÇÃO:
             *
             * O produto é criado com o preço unitário bruto da compra,
             * mas o estoque nasce em ZERO.
             *
             * A quantidade da planilha será registrada no
             * EntradaCompraItem.
             *
             * Somente POST /confirmar movimentará o estoque.
             */
            var produto =
                new Produto(
                    linha.Nome,
                    linha.Tipo,
                    linha.PrecoCompra,
                    linha.PrecoVenda,
                    linha.Descricao,
                    linha.CodigoSku!.Trim(),
                    linha.Marca,
                    linha.Modelo,
                    linha.CodigoFornecedor,
                    linha.CodigoBarras,
                    linha.Ncm,
                    linha.UnidadeComercial);

            var estoque =
                new Estoque(
                    produto.Id,
                    quantidadeInicial: 0,
                    linha.QuantidadeMinima);

            entrada.AdicionarItem(
                produto.Id,
                produto.CodigoSku,
                produto.CodigoFornecedor,
                produto.Nome,
                linha.QuantidadeInicial,
                linha.PrecoCompra);

            produtos.Add(produto);
            estoques.Add(estoque);
        }

        /*
         * Persistimos catálogo + estoques zerados + entrada + itens
         * em um único SaveChanges.
         *
         * Nenhuma MovimentacaoEstoque é criada nesta etapa.
         */
        await produtoRepository.AddRangeAsync(
            produtos,
            ct);

        await estoqueRepository.AddRangeAsync(
            estoques,
            ct);

        await entradaRepository.AdicionarAsync(
            entrada,
            ct);

        await uow.CommitAsync(ct);

        var entradaPersistida =
            await entradaRepository.ObterDetalheAsync(
                entrada.Id,
                ct)
            ?? throw new InvalidOperationException(
                "A entrada foi criada, mas não pôde ser recarregada.");

        logger.LogInformation(
            "Importação de entrada concluída em rascunho. " +
            "EntradaId: {EntradaId}. Produtos: {Produtos}.",
            entrada.Id,
            produtos.Count);

        return new EntradaCompraImportacaoResultadoDto
        {
            Sucesso = true,
            NomeArquivo = sanitizedFileName,
            TotalLinhas = leitura.TotalLinhas,
            ProdutosCriados = produtos.Count,
            EntradaCompraId = entrada.Id,
            Entrada =
                EntradaCompraMapper.ToDetalheDto(
                    entradaPersistida),
            Erros =
                Array.Empty<ProdutoImportacaoErroDto>()
        };
    }

    private static EntradaCompraImportacaoResultadoDto
        CriarResultadoErro(
            string nomeArquivo,
            int totalLinhas,
            IEnumerable<ProdutoImportacaoErroDto> erros)
    {
        var lista =
            erros
                .OrderBy(x => x.Linha)
                .ThenBy(x => x.Campo)
                .ToList();

        return new EntradaCompraImportacaoResultadoDto
        {
            Sucesso = false,
            NomeArquivo = nomeArquivo,
            TotalLinhas = totalLinhas,
            ProdutosCriados = 0,
            EntradaCompraId = null,
            Entrada = null,
            Erros = lista
        };
    }
}
