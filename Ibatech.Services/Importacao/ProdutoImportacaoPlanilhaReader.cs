using Ibatech.Domain.DTOs;
using Ibatech.Services.DTOs.Produto;
using MiniExcelLibs;

namespace Ibatech.Services.Importacao;

internal sealed class ProdutoImportacaoPlanilhaReader
{
    private const long MaxFileSize = 5 * 1024 * 1024; // 5 MB
    private const int MaxRows = 500;

    public async Task<ProdutoImportacaoLeituraResultado> LerAsync(
        Stream arquivo,
        string nomeArquivo,
        long tamanhoArquivo,
        CancellationToken cancellationToken = default)
    {
        var erros = new List<ProdutoImportacaoErroDto>();

        // Validações de arquivo
        if (arquivo == null || !arquivo.CanRead)
            erros.Add(new ProdutoImportacaoErroDto
            {
                Linha = 0,
                Campo = "Arquivo",
                Mensagem = "O arquivo é inválido ou não pode ser lido."
            });
        else if (tamanhoArquivo <= 0 || tamanhoArquivo > MaxFileSize)
            erros.Add(new ProdutoImportacaoErroDto
            {
                Linha = 0,
                Campo = "Arquivo",
                Mensagem = "O arquivo deve ter entre 1 byte e 5 MB."
            });

        if (string.IsNullOrWhiteSpace(nomeArquivo))
            erros.Add(new ProdutoImportacaoErroDto
            {
                Linha = 0,
                Campo = "Arquivo",
                Mensagem = "O nome do arquivo é obrigatório."
            });
        else if (!nomeArquivo.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
            erros.Add(new ProdutoImportacaoErroDto
            {
                Linha = 0,
                Campo = "Arquivo",
                Valor = nomeArquivo,
                Mensagem = "O arquivo deve ser um .xlsx."
            });

        if (erros.Any())
            return new ProdutoImportacaoLeituraResultado
            {
                Sucesso = false,
                Erros = erros
            };

        try
        {
            var sheetNames = arquivo.GetSheetNames();

            if (sheetNames == null || !sheetNames.Any())
                return CriarErroArquivo("A planilha não contém abas.");

            /*
             * IMPORTANTE:
             * Com useHeaderRow: true o MiniExcel utiliza a primeira linha
             * física como cabeçalho e os objetos retornados já representam
             * as linhas de dados.
             *
             * Portanto NÃO devemos fazer Skip(1), pois isso descartaria
             * o primeiro produto da planilha.
             */
            var rows = arquivo
                .Query(
                    useHeaderRow: true,
                    sheetName: sheetNames.First())
                .ToList();

            if (rows.Count == 0)
                return CriarErroArquivo(
                    "A planilha não contém linhas de dados.");

            // As chaves da primeira linha de dados representam os cabeçalhos.
            var firstRow = rows.First() as IDictionary<string, object>;

            var headers = firstRow?.Keys
                .Select(k => k?.ToString()?.Trim())
                .ToList()
                ?? new List<string?>();

            var requiredHeaders = new[]
            {
                "Nome",
                "Tipo",
                "PrecoCompra",
                "PrecoVenda",
                "QuantidadeInicial"
            };

            var allAllowedHeaders = requiredHeaders
                .Concat(new[]
                {
                    "QuantidadeMinima",
                    "CodigoSku",
                    "CodigoFornecedor",
                    "CodigoBarras",
                    "Ncm",
                    "UnidadeComercial",
                    "Descricao",
                    "Marca",
                    "Modelo"
                })
                .ToList();

            foreach (var req in requiredHeaders)
            {
                if (!headers.Any(h =>
                        string.Equals(
                            h,
                            req,
                            StringComparison.OrdinalIgnoreCase)))
                {
                    erros.Add(new ProdutoImportacaoErroDto
                    {
                        Linha = 1,
                        Campo = "Cabecalho",
                        Valor = req,
                        Mensagem = $"Cabeçalho obrigatório ausente: {req}"
                    });
                }
            }

            foreach (var h in headers)
            {
                if (!string.IsNullOrWhiteSpace(h) &&
                    !allAllowedHeaders.Any(a =>
                        string.Equals(
                            a,
                            h,
                            StringComparison.OrdinalIgnoreCase)))
                {
                    erros.Add(new ProdutoImportacaoErroDto
                    {
                        Linha = 1,
                        Campo = "Cabecalho",
                        Valor = h,
                        Mensagem = $"Cabeçalho desconhecido: {h}"
                    });
                }
            }

            if (erros.Any())
                return new ProdutoImportacaoLeituraResultado
                {
                    Sucesso = false,
                    Erros = erros
                };

            var linhas = new List<ProdutoImportacaoLinhaDto>();
            var nonEmptyCount = 0;

            for (var i = 0; i < rows.Count; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var row = rows[i] as IDictionary<string, object>;

                if (row == null ||
                    row.Values.All(v =>
                        v == null ||
                        v == DBNull.Value ||
                        string.IsNullOrWhiteSpace(v.ToString())))
                {
                    continue;
                }

                nonEmptyCount++;

                if (nonEmptyCount > MaxRows)
                    return CriarErroArquivo(
                        $"Limite de {MaxRows} linhas de dados excedido.");

                linhas.Add(new ProdutoImportacaoLinhaDto
                {
                    // Linha física do Excel:
                    // 1 = cabeçalho; dados começam em 2.
                    NumeroLinha = i + 2,

                    Nome = GetValue(row, "Nome"),
                    Tipo = GetValue(row, "Tipo"),
                    PrecoCompra = GetValue(row, "PrecoCompra"),
                    PrecoVenda = GetValue(row, "PrecoVenda"),

                    QuantidadeInicial =
                        GetValue(row, "QuantidadeInicial"),

                    QuantidadeMinima =
                        GetValue(row, "QuantidadeMinima"),

                    CodigoSku = GetValue(row, "CodigoSku"),

                    CodigoFornecedor =
                        GetValue(row, "CodigoFornecedor"),

                    CodigoBarras =
                        GetValue(row, "CodigoBarras"),

                    Ncm = GetValue(row, "Ncm"),

                    UnidadeComercial =
                        GetValue(row, "UnidadeComercial"),

                    Descricao = GetValue(row, "Descricao"),
                    Marca = GetValue(row, "Marca"),
                    Modelo = GetValue(row, "Modelo")
                });
            }

            if (nonEmptyCount == 0)
                return CriarErroArquivo(
                    "A planilha não contém linhas de dados.");

            return new ProdutoImportacaoLeituraResultado
            {
                Sucesso = true,
                TotalLinhas = nonEmptyCount,
                Linhas = linhas
            };
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch
        {
            return CriarErroArquivo(
                "Não foi possível ler a planilha. " +
                "Verifique se o arquivo é um .xlsx válido.");
        }
    }

    private static object? GetValue(
        IDictionary<string, object> row,
        string header)
    {
        var key = row.Keys.FirstOrDefault(k =>
            string.Equals(
                k,
                header,
                StringComparison.OrdinalIgnoreCase));

        return key != null ? row[key] : null;
    }

    private static ProdutoImportacaoLeituraResultado CriarErroArquivo(
        string msg) =>
        new()
        {
            Sucesso = false,
            Erros =
            [
                new ProdutoImportacaoErroDto
                {
                    Linha = 0,
                    Campo = "Arquivo",
                    Mensagem = msg
                }
            ]
        };
}
