using Ibatech.Domain.DTOs;
using Ibatech.Domain.Entities;
using Ibatech.Domain.Interfaces.Repositories;
using Ibatech.Domain.Interfaces.Services;
using Ibatech.Repository.UnitOfWork;
using Ibatech.Services.DTOs.Produto;
using Ibatech.Services.Importacao;
using Microsoft.Extensions.Logging;

namespace Ibatech.Services.Implementations;

public sealed class ProdutoImportacaoService(
    IProdutoRepository produtoRepository,
    IEstoqueRepository estoqueRepository,
    IUnitOfWork unitOfWork,
    ILogger<ProdutoImportacaoService> logger)
    : IProdutoImportacaoService
{
    public async Task<ProdutoImportacaoResultadoDto> ImportarAsync(
        Stream arquivo,
        string nomeArquivo,
        long tamanhoArquivo,
        CancellationToken cancellationToken = default)
    {
        var sanitizedFileName = Path.GetFileName(nomeArquivo);
        logger.LogInformation("Iniciando importação do arquivo: {FileName}", sanitizedFileName);

        var reader = new ProdutoImportacaoPlanilhaReader();
        var leitura = await reader.LerAsync(arquivo, nomeArquivo, tamanhoArquivo, cancellationToken);

        if (!leitura.Sucesso)
        {
            return CriarResultadoErro(sanitizedFileName, leitura.TotalLinhas, leitura.Erros.ToList());
        }

        var validador = new ProdutoImportacaoLinhaValidador();
        var validacao = validador.Validar(leitura.Linhas!);

        if (!validacao.Sucesso)
        {
            return CriarResultadoErro(sanitizedFileName, leitura.TotalLinhas, validacao.Erros.ToList());
        }

        var erros = new List<ProdutoImportacaoErroDto>();
        var linhasValidadas = validacao.Linhas;

        // 1. Detectar duplicados na planilha
        var skusPlanilha = linhasValidadas
            .Where(l => !string.IsNullOrWhiteSpace(l.CodigoSku))
            .GroupBy(l => l.CodigoSku!.Trim(), StringComparer.OrdinalIgnoreCase)
            .ToList();

        foreach (var grupo in skusPlanilha.Where(g => g.Count() > 1))
        {
            foreach (var linha in grupo)
            {
                erros.Add(new ProdutoImportacaoErroDto
                {
                    Linha = linha.NumeroLinha,
                    Campo = "CodigoSku",
                    Valor = linha.CodigoSku,
                    Mensagem = "O SKU está duplicado na própria planilha."
                });
            }
        }

        if (erros.Any())
        {
            return CriarResultadoErro(sanitizedFileName, leitura.TotalLinhas, erros);
        }

        // 2. Detectar duplicados no banco
        var skusParaVerificar = linhasValidadas
            .Where(l => !string.IsNullOrWhiteSpace(l.CodigoSku))
            .Select(l => l.CodigoSku!.Trim())
            .Distinct()
            .ToList();

        if (skusParaVerificar.Any())
        {
            var skusExistentes = await produtoRepository.ObterSkusExistentesAsync(skusParaVerificar, cancellationToken);
            foreach (var linha in linhasValidadas.Where(l => !string.IsNullOrWhiteSpace(l.CodigoSku)))
            {
                if (skusExistentes.Contains(linha.CodigoSku!.Trim(), StringComparer.OrdinalIgnoreCase))
                {
                    erros.Add(new ProdutoImportacaoErroDto
                    {
                        Linha = linha.NumeroLinha,
                        Campo = "CodigoSku",
                        Valor = linha.CodigoSku,
                        Mensagem = "Já existe um produto cadastrado com este SKU."
                    });
                }
            }
        }

        if (erros.Any())
        {
            return CriarResultadoErro(sanitizedFileName, leitura.TotalLinhas, erros);
        }

        // 3. Criar entidades
        var produtos = new List<Produto>();
        var estoques = new List<Estoque>();

        foreach (var linha in linhasValidadas)
        {
            var produto = new Produto(
                linha.Nome!,
                linha.Tipo,
                linha.PrecoCompra,
                linha.PrecoVenda,
                linha.Descricao,
                linha.CodigoSku?.Trim(),
                linha.Marca,
                linha.Modelo
            );

            var estoque = new Estoque(
                produto.Id,
                linha.QuantidadeInicial,
                linha.QuantidadeMinima
            );

            produtos.Add(produto);
            estoques.Add(estoque);
        }

        // 4. Persistir
        await produtoRepository.AddRangeAsync(produtos, cancellationToken);
        await estoqueRepository.AddRangeAsync(estoques, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);

        logger.LogInformation("Importação concluída com sucesso. {Count} produtos importados.", produtos.Count);

        return new ProdutoImportacaoResultadoDto
        {
            Sucesso = true,
            NomeArquivo = sanitizedFileName,
            TotalLinhas = leitura.TotalLinhas,
            LinhasValidas = leitura.TotalLinhas,
            LinhasComErro = 0,
            ProdutosImportados = produtos.Count,
            Erros = new List<ProdutoImportacaoErroDto>()
        };
    }

    private static ProdutoImportacaoResultadoDto CriarResultadoErro(string nomeArquivo, int totalLinhas, List<ProdutoImportacaoErroDto> erros)
    {
        var linhasComErro = erros.Select(e => e.Linha).Distinct().Count();
        return new ProdutoImportacaoResultadoDto
        {
            Sucesso = false,
            NomeArquivo = nomeArquivo,
            TotalLinhas = totalLinhas,
            LinhasValidas = totalLinhas - linhasComErro,
            LinhasComErro = linhasComErro,
            ProdutosImportados = 0,
            Erros = erros.OrderBy(e => e.Linha).ThenBy(e => e.Campo).ToList()
        };
    }
}
