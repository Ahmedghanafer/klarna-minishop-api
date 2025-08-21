using Minishop.Core.Entities;

namespace Minishop.Application.Auth.Interfaces;

public interface IRefreshTokenStore
{
    Task StoreAsync(Guid userId, string tokenHash, DateTime expiresAtUtc, CancellationToken ct = default);
    Task<RefreshToken?> GetValidByHashAsync(string tokenHash, CancellationToken ct = default);
    Task RevokeAsync(Guid refreshTokenId, CancellationToken ct = default);
}