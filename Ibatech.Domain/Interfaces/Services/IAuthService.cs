using Ibatech.Domain.DTOs;

namespace Ibatech.Domain.Interfaces.Services;

public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(LoginRequestDto dto, CancellationToken ct = default);
}
