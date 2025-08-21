using Minishop.Application.Auth.DTOs;

namespace Minishop.Application.Auth.Interfaces;

public interface IAuthService
{
    Task RegisterAsync(RegisterRequest request, CancellationToken ct = default);
    Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken ct = default);
    Task<AccessTokenResponse> RefreshAsync(RefreshRequest request, CancellationToken ct = default);
}