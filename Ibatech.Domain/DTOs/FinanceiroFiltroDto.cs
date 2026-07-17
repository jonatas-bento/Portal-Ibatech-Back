namespace Ibatech.Domain.DTOs;

public class FinanceiroFiltroDto
{
    public Guid? VendaId { get; set; }
    public int Pagina { get; set; } = 1;
    public int TamanhoPagina { get; set; } = 10;
    public DateTime? DataInicio { get; set; }
    public DateTime? DataFim { get; set; }

    public Ibatech.Domain.Enums.TipoTransacao? Tipo { get; set; }
    public string? Categoria { get; set; }
    public bool? Liquidada { get; set; }
    public Ibatech.Domain.Enums.FormaPagamento? FormaPagamento { get; set; }
    public string? Termo { get; set; }
}
