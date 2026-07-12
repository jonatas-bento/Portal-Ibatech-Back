using Ibatech.Domain.Entities;
using Ibatech.Domain.DTOs;

namespace Ibatech.Services.Mappers;

public static class ClienteMapper
{
    public static ClienteResumoDto ToResumoDto(this Cliente cliente) =>
        new(cliente.Id, cliente.Nome, cliente.CpfCnpj, cliente.Email, cliente.Ativo);

    public static IEnumerable<ClienteResumoDto> ToResumoDtoList(this IEnumerable<Cliente> clientes) =>
        clientes.Select(c => c.ToResumoDto());

    public static ClienteDetalheDto ToDetalheDto(this Cliente cliente) =>
        new(cliente.Id, cliente.Nome, cliente.CpfCnpj, cliente.Telefone, cliente.Email, cliente.Endereco, cliente.Observacao, cliente.Ativo);
}
