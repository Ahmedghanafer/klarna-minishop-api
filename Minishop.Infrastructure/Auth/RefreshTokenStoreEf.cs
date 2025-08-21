using Microsoft.EntityFrameworkCore;
using Minishop.Application.Auth.Interfaces;
using Minishop.Core.Entities;
using Minishop.Infrastructure.Persistence;

namespace Minishop.Infrastructure.Auth;

public sealed class RefreshTokenStoreEf : IRefreshTokenStore
{
    private readonly ApplicationDbContext _db;
    public RefreshTokenStoreEf(ApplicationDbContext db) => _db = db;

    public async Task StoreAsync(Guid userId, string tokenHash, DateTime expiresAtUtc, CancellationToken ct = default)
    {
        var entity = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            TokenHash = tokenHash,
            ExpiresAtUtc = expiresAtUtc
        };
        _db.RefreshTokens.Add(entity);
        await _db.SaveChangesAsync(ct);
    }

    public async Task<RefreshToken?> GetValidByHashAsync(string tokenHash, CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        return await _db.RefreshTokens
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
                x.TokenHash == tokenHash &&
                x.RevokedAtUtc == null &&
                x.ExpiresAtUtc > now, ct);
    }

    public async Task RevokeAsync(Guid refreshTokenId, CancellationToken ct = default)
    {
        await _db.RefreshTokens
            .Where(x => x.Id == refreshTokenId)
            .ExecuteUpdateAsync(s => s.SetProperty(x => x.RevokedAtUtc, DateTime.UtcNow), ct);
    }
}