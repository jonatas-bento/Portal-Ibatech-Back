using Ibatech.Domain.DTOs;
using Ibatech.Domain.Entities;

namespace Ibatech.Services.Mappers;

public static class FornecedorMapper
{
    public static FornecedorDto ToDto(Fornecedor fornecedor) =>
        new(
            fornecedor.Id,
            fornecedor.Nome,
            fornecedor.NomeFantasia,
            fornecedor.Documento,
            fornecedor.Email,
            fornecedor.Telefone,
            fornecedor.Observacao,
            fornecedor.Ativo);
}
