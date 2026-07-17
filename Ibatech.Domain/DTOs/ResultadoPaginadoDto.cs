namespace Ibatech.Domain.DTOs;

public sealed class ResultadoPaginadoDto<T>
{
    public IReadOnlyCollection<T> Itens { get; init; } = [];
    public int Pagina { get; init; }
    public int TamanhoPagina { get; init; }
    public int TotalItens { get; init; }
    public int TotalPaginas { get; init; }
}
