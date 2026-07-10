namespace Ibatech.Services.DTOs.Financeiro;

public sealed record ResumoFinanceiroDto(
    decimal TotalReceitas,
    decimal TotalDespesas,
    decimal Saldo,
    decimal ReceitasPendentes,
    decimal DespesasPendentes,
    int TransacoesVencidas
);