using System.Globalization;
using System.Text;
using Ibatech.Domain.DTOs;
using Ibatech.Domain.Enums;
using Ibatech.Services.DTOs.Produto;

namespace Ibatech.Services.Importacao;

internal sealed class ProdutoImportacaoLinhaValidador
{
    public ProdutoImportacaoValidacaoResultado Validar(
        IReadOnlyList<ProdutoImportacaoLinhaDto> linhas)
    {
        var validas = new List<ProdutoImportacaoLinhaValidada>();
        var erros = new List<ProdutoImportacaoErroDto>();
        var linhasComErro = new HashSet<int>();

        foreach (var linha in linhas)
        {
            var linhaErros = new List<ProdutoImportacaoErroDto>();

            // Nome
            var nome = NormalizarString(linha.Nome);

            if (string.IsNullOrWhiteSpace(nome))
            {
                linhaErros.Add(
                    CriarErro(
                        linha.NumeroLinha,
                        "Nome",
                        linha.Nome,
                        "O nome é obrigatório."));
            }
            else if (nome.Length > 200)
            {
                linhaErros.Add(
                    CriarErro(
                        linha.NumeroLinha,
                        "Nome",
                        nome,
                        "O nome deve ter no máximo 200 caracteres."));
            }

            // Tipo
            var tipo = ValidarTipo(
                linha.Tipo,
                linha.NumeroLinha,
                linhaErros);

            // Preços
            var precoCompra = ValidarDecimal(
                linha.PrecoCompra,
                linha.NumeroLinha,
                "PrecoCompra",
                "Preço de compra",
                linhaErros,
                true);

            var precoVenda = ValidarDecimal(
                linha.PrecoVenda,
                linha.NumeroLinha,
                "PrecoVenda",
                "Preço de venda",
                linhaErros,
                true);

            // Quantidades
            var qtdInicial = ValidarInt(
                linha.QuantidadeInicial,
                linha.NumeroLinha,
                "QuantidadeInicial",
                "Quantidade inicial",
                linhaErros,
                true);

            var qtdMinima = ValidarInt(
                linha.QuantidadeMinima,
                linha.NumeroLinha,
                "QuantidadeMinima",
                "Quantidade mínima",
                linhaErros,
                false,
                5);

            // Identificadores
            var sku = NormalizarString(linha.CodigoSku);

            ValidarTamanho(
                sku,
                50,
                linha.NumeroLinha,
                "CodigoSku",
                "O SKU deve ter no máximo 50 caracteres.",
                linhaErros);

            var codigoFornecedor =
                NormalizarString(linha.CodigoFornecedor);

            ValidarTamanho(
                codigoFornecedor,
                100,
                linha.NumeroLinha,
                "CodigoFornecedor",
                "O código do fornecedor deve ter no máximo 100 caracteres.",
                linhaErros);

            var codigoBarras =
                NormalizarString(linha.CodigoBarras);

            ValidarTamanho(
                codigoBarras,
                50,
                linha.NumeroLinha,
                "CodigoBarras",
                "O código de barras deve ter no máximo 50 caracteres.",
                linhaErros);

            var ncm = NormalizarString(linha.Ncm);

            ValidarTamanho(
                ncm,
                20,
                linha.NumeroLinha,
                "Ncm",
                "O NCM deve ter no máximo 20 caracteres.",
                linhaErros);

            var unidadeComercial =
                NormalizarString(linha.UnidadeComercial) ?? "UN";

            ValidarTamanho(
                unidadeComercial,
                10,
                linha.NumeroLinha,
                "UnidadeComercial",
                "A unidade comercial deve ter no máximo 10 caracteres.",
                linhaErros);

            if (linhaErros.Any())
            {
                erros.AddRange(linhaErros);
                linhasComErro.Add(linha.NumeroLinha);
                continue;
            }

            validas.Add(new ProdutoImportacaoLinhaValidada
            {
                NumeroLinha = linha.NumeroLinha,

                Nome = nome!,
                Tipo = tipo,

                PrecoCompra = precoCompra,
                PrecoVenda = precoVenda,

                QuantidadeInicial = qtdInicial,
                QuantidadeMinima = qtdMinima,

                CodigoSku = sku,
                CodigoFornecedor = codigoFornecedor,
                CodigoBarras = codigoBarras,
                Ncm = ncm,
                UnidadeComercial = unidadeComercial,

                Descricao =
                    NormalizarString(linha.Descricao),

                Marca =
                    NormalizarString(linha.Marca),

                Modelo =
                    NormalizarString(linha.Modelo)
            });
        }

        return new ProdutoImportacaoValidacaoResultado
        {
            Sucesso = !erros.Any(),
            TotalLinhas = linhas.Count,
            LinhasValidas = validas.Count,
            LinhasComErro = linhasComErro.Count,
            Linhas = validas,
            Erros = erros
                .OrderBy(e => e.Linha)
                .ThenBy(e => e.Campo)
                .ToList()
        };
    }

    private static void ValidarTamanho(
        string? valor,
        int tamanhoMaximo,
        int linha,
        string campo,
        string mensagem,
        List<ProdutoImportacaoErroDto> erros)
    {
        if (valor?.Length > tamanhoMaximo)
        {
            erros.Add(
                CriarErro(
                    linha,
                    campo,
                    valor,
                    mensagem));
        }
    }

    private static string? NormalizarString(object? valor)
    {
        var s = valor?.ToString()?.Trim();

        return string.IsNullOrEmpty(s)
            ? null
            : s;
    }

    private static TipoProduto ValidarTipo(
        object? valor,
        int linha,
        List<ProdutoImportacaoErroDto> erros)
    {
        var s = NormalizarString(valor);

        if (string.IsNullOrEmpty(s))
        {
            erros.Add(
                CriarErro(
                    linha,
                    "Tipo",
                    valor,
                    "O tipo é obrigatório."));

            return default;
        }

        var normalizado = RemoverAcentos(s)
            .Replace(" ", "")
            .ToLowerInvariant();

        if (normalizado == "computador")
            return TipoProduto.Computador;

        if (normalizado == "peca")
            return TipoProduto.Peca;

        if (normalizado == "acessoriomovel")
            return TipoProduto.AcessorioMovel;

        if (normalizado == "periferico")
            return TipoProduto.Periferico;

        erros.Add(
            CriarErro(
                linha,
                "Tipo",
                s,
                "Tipo inválido."));

        return default;
    }

    private static decimal ValidarDecimal(
        object? valor,
        int linha,
        string campo,
        string nomeCampo,
        List<ProdutoImportacaoErroDto> erros,
        bool obrigatorio)
    {
        var s = NormalizarString(valor);

        if (string.IsNullOrEmpty(s))
        {
            if (obrigatorio)
            {
                erros.Add(
                    CriarErro(
                        linha,
                        campo,
                        valor,
                        $"{nomeCampo} é obrigatório."));
            }

            return 0;
        }

        var formatado = s
            .Replace(
                ".",
                CultureInfo.InvariantCulture
                    .NumberFormat.NumberDecimalSeparator)
            .Replace(
                ",",
                CultureInfo.InvariantCulture
                    .NumberFormat.NumberDecimalSeparator);

        if (decimal.TryParse(
                formatado,
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out var d) &&
            d >= 0)
        {
            if (decimal.Round(d, 2) != d)
            {
                erros.Add(
                    CriarErro(
                        linha,
                        campo,
                        s,
                        $"{nomeCampo} não pode ter mais de duas casas decimais."));
            }

            return d;
        }

        erros.Add(
            CriarErro(
                linha,
                campo,
                s,
                $"{nomeCampo} inválido."));

        return 0;
    }

    private static int ValidarInt(
        object? valor,
        int linha,
        string campo,
        string nomeCampo,
        List<ProdutoImportacaoErroDto> erros,
        bool obrigatorio,
        int? valorPadrao = null)
    {
        var s = NormalizarString(valor);

        if (string.IsNullOrEmpty(s))
        {
            if (obrigatorio)
            {
                erros.Add(
                    CriarErro(
                        linha,
                        campo,
                        valor,
                        $"{nomeCampo} é obrigatório."));
            }

            return valorPadrao ?? 0;
        }

        if (int.TryParse(s, out var i) && i >= 0)
            return i;

        erros.Add(
            CriarErro(
                linha,
                campo,
                s,
                $"{nomeCampo} inválido."));

        return 0;
    }

    private static string RemoverAcentos(string texto)
    {
        var normalizado =
            texto.Normalize(NormalizationForm.FormD);

        var sb =
            new StringBuilder(capacity: normalizado.Length);

        foreach (var c in normalizado)
        {
            var categoria =
                CharUnicodeInfo.GetUnicodeCategory(c);

            if (categoria !=
                UnicodeCategory.NonSpacingMark)
            {
                sb.Append(c);
            }
        }

        return sb
            .ToString()
            .Normalize(NormalizationForm.FormC);
    }

    private static ProdutoImportacaoErroDto CriarErro(
        int linha,
        string campo,
        object? valor,
        string msg) =>
        new()
        {
            Linha = linha,
            Campo = campo,
            Valor = valor?.ToString(),
            Mensagem = msg
        };
}
