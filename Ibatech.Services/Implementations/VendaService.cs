using Ibatech.Domain.DTOs;
using Ibatech.Domain.Entities;
using Ibatech.Domain.Enums;
using Ibatech.Domain.Interfaces.Repositories;
using Ibatech.Domain.Interfaces.Services;
using Ibatech.Repository.UnitOfWork;
using Ibatech.Services.Mappers;

namespace Ibatech.Services.Implementations;

public sealed class VendaService(
    IVendaRepository vendaRepository,
    IClienteRepository clienteRepository,
    IProdutoRepository produtoRepository,
    IUsuarioRepository usuarioRepository,
    IUnitOfWork uow) : IVendaService
{
    public async Task<IReadOnlyCollection<VendaResumoDto>> ListarAsync(
        VendaFiltroDto filtros,
        CancellationToken cancellationToken = default)
    {
        var vendas = await vendaRepository.ListarAsync(filtros, cancellationToken);
        return vendas.Select(VendaMapper.ToResumoDto).ToList();
    }

    public async Task<VendaDetalheDto> ObterPorIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty) throw new ArgumentException("ID inválido.");

        var venda = await vendaRepository.ObterDetalheAsync(id, cancellationToken);
        if (venda is null) throw new KeyNotFoundException("Venda não encontrada.");

        return VendaMapper.ToDetalheDto(venda);
    }

    public async Task<VendaDetalheDto> CriarAsync(
        CriarVendaDto dto,
        Guid vendedorId,
        CancellationToken cancellationToken = default)
    {
        if (vendedorId == Guid.Empty) throw new ArgumentException("Vendedor inválido.");

        var vendedor = await usuarioRepository.ObterPorIdAsync(vendedorId);
        if (vendedor is null || !vendedor.Ativo)
            throw new InvalidOperationException("Vendedor não encontrado ou inativo.");

        if (vendedor.Role != RoleUsuario.Admin && vendedor.Role != RoleUsuario.Vendedor)
            throw new InvalidOperationException("Usuário sem permissão para criar vendas.");

        Guid? clienteId = null;
        string? clienteNome = null;
        string? clienteCpfCnpj = null;

        if (dto.ClienteId.HasValue)
        {
            var cliente = await clienteRepository.ObterPorIdAsync(dto.ClienteId.Value);
            if (cliente is null || !cliente.Ativo)
                throw new InvalidOperationException("Cliente não encontrado ou inativo.");

            clienteId = cliente.Id;
            clienteNome = cliente.Nome;
            clienteCpfCnpj = cliente.CpfCnpj;
        }

        var numero = await GerarNumeroUnicoAsync(cancellationToken);

        var venda = new Venda(
            numero,
            clienteId,
            clienteNome,
            clienteCpfCnpj,
            vendedor.Id,
            vendedor.NomeCompleto,
            dto.Observacao);

        await vendaRepository.AdicionarAsync(venda);
        await uow.CommitAsync(cancellationToken);

        return VendaMapper.ToDetalheDto(venda);
    }

    public async Task<VendaDetalheDto> AtualizarAsync(
        Guid id,
        AtualizarVendaDto dto,
        CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty) throw new ArgumentException("ID da venda inválido.");

        var venda = await vendaRepository.ObterComItensAsync(id, cancellationToken);
        if (venda is null) throw new KeyNotFoundException("Venda não encontrada.");

        Guid? clienteId = null;
        string? clienteNome = null;
        string? clienteCpfCnpj = null;

        if (dto.ClienteId.HasValue)
        {
            var cliente = await clienteRepository.ObterPorIdAsync(dto.ClienteId.Value);
            if (cliente is null || !cliente.Ativo)
                throw new InvalidOperationException("Cliente não encontrado ou inativo.");

            clienteId = cliente.Id;
            clienteNome = cliente.Nome;
            clienteCpfCnpj = cliente.CpfCnpj;
        }

        venda.AtualizarCabecalho(clienteId, clienteNome, clienteCpfCnpj, dto.Observacao);

        await uow.CommitAsync(cancellationToken);
        return VendaMapper.ToDetalheDto(venda);
    }

    public async Task<VendaDetalheDto> AdicionarItemAsync(
        Guid vendaId,
        AdicionarVendaItemDto dto,
        CancellationToken cancellationToken = default)
    {
        if (vendaId == Guid.Empty) throw new ArgumentException("ID da venda inválido.");
        if (dto.ProdutoId == Guid.Empty) throw new ArgumentException("ID do produto inválido.");

        var venda = await vendaRepository.ObterComItensAsync(vendaId, cancellationToken);
        if (venda is null) throw new KeyNotFoundException("Venda não encontrada.");

        var produto = await produtoRepository.ObterPorIdAsync(dto.ProdutoId);
        if (produto is null || !produto.Ativo)
            throw new InvalidOperationException("Produto não encontrado ou inativo.");

        venda.AdicionarItem(
            produto.Id,
            produto.CodigoSku,
            produto.Nome,
            produto.Descricao,
            dto.Quantidade,
            produto.PrecoVenda,
            dto.Desconto);

        await uow.CommitAsync(cancellationToken);
        return VendaMapper.ToDetalheDto(venda);
    }

    public async Task<VendaDetalheDto> AtualizarItemAsync(
        Guid vendaId,
        Guid itemId,
        AtualizarVendaItemDto dto,
        CancellationToken cancellationToken = default)
    {
        if (vendaId == Guid.Empty) throw new ArgumentException("ID da venda inválido.");
        if (itemId == Guid.Empty) throw new ArgumentException("ID da venda inválido.");

        var venda = await vendaRepository.ObterComItensAsync(vendaId, cancellationToken);
        if (venda is null) throw new KeyNotFoundException("Venda não encontrada.");

        venda.AtualizarItem(itemId, dto.Quantidade, dto.Desconto);

        await uow.CommitAsync(cancellationToken);
        return VendaMapper.ToDetalheDto(venda);
    }

    public async Task<VendaDetalheDto> RemoverItemAsync(
        Guid vendaId,
        Guid itemId,
        CancellationToken cancellationToken = default)
    {
        if (vendaId == Guid.Empty) throw new ArgumentException("ID da venda inválido.");
        if (itemId == Guid.Empty) throw new ArgumentException("ID da venda inválido.");

        var venda = await vendaRepository.ObterComItensAsync(vendaId, cancellationToken);
        if (venda is null) throw new KeyNotFoundException("Venda não encontrada.");

        venda.RemoverItem(itemId);

        await uow.CommitAsync(cancellationToken);
        return VendaMapper.ToDetalheDto(venda);
    }

    private async Task<string> GerarNumeroUnicoAsync(CancellationToken ct)
    {
        for (int i = 0; i < 5; i++)
        {
            var sufixo = Guid.NewGuid().ToString("N")[..10].ToUpperInvariant();
            var numero = $"VD-{DateTime.UtcNow:yyMMdd}-{sufixo}";

            if (!await vendaRepository.ExisteNumeroAsync(numero, ct))
                return numero;
        }

        throw new InvalidOperationException("Não foi possível gerar um número de venda único após várias tentativas.");
    }
}
