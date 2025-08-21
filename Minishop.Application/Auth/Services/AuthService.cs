using System.Security.Cryptography;
using System.Text;
using Minishop.Application.Auth.DTOs;
using Minishop.Application.Auth.Interfaces;
using Minishop.Core.Entities;

namespace Minishop.Application.Auth.Services;

public sealed class AuthService : IAuthService
{
    private readonly IUserRepository _users;
    private readonly IPasswordHasher _hasher;
    private readonly IJwtTokenService _jwt;
    private readonly IRefreshTokenStore _refreshStore;

    private static readonly TimeSpan AccessTokenLifetime = TimeSpan.FromMinutes(15);
    private static readonly TimeSpan RefreshTokenLifetime = TimeSpan.FromDays(7);

    public AuthService(
        IUserRepository users,
        IPasswordHasher hasher,
        IJwtTokenService jwt,
        IRefreshTokenStore refreshStore)
    {
        _users = users;
        _hasher = hasher;
        _jwt = jwt;
        _refreshStore = refreshStore;
    }

    public async Task RegisterAsync(RegisterRequest request, CancellationToken ct = default)
    {
        var email = NormalizeEmail(request.Email);
        ValidateEmail(email);
        ValidatePassword(request.Password);

        if (await _users.ExistsByEmailAsync(email, ct))
            throw new InvalidOperationException("Email already registered.");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            PasswordHash = _hasher.Hash(request.Password),
            Role = UserRole.Customer
        };

        await _users.AddAsync(user, ct);
        await _users.SaveChangesAsync(ct);
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        var email = NormalizeEmail(request.Email);
        var user = await _users.GetByEmailAsync(email, ct);
        if (user is null || !_hasher.Verify(request.Password, user.PasswordHash))
            throw new InvalidOperationException("Invalid email or password.");

        var access = _jwt.CreateAccessToken(user, AccessTokenLifetime);

        var refreshPlain = _jwt.CreateRefreshTokenPlaintext();
        var refreshHash = Sha256Base64(refreshPlain);
        await _refreshStore.StoreAsync(user.Id, refreshHash, DateTime.UtcNow.Add(RefreshTokenLifetime), ct);

        return new LoginResponse { AccessToken = access, RefreshToken = refreshPlain };
    }

    public async Task<AccessTokenResponse> RefreshAsync(RefreshRequest request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
            throw new UnauthorizedAccessException("Missing refresh token.");

        var hash = Sha256Base64(request.RefreshToken);
        var record = await _refreshStore.GetValidByHashAsync(hash, ct);
        if (record is null)
            throw new UnauthorizedAccessException("Invalid or expired refresh token.");

        // single-use: revoke the current token
        await _refreshStore.RevokeAsync(record.Id, ct);

        var user = await _users.GetByIdAsync(record.UserId, ct)
                   ?? throw new UnauthorizedAccessException("User no longer exists.");

        var access = _jwt.CreateAccessToken(user, AccessTokenLifetime);
        return new AccessTokenResponse { AccessToken = access };
    }

    private static string NormalizeEmail(string email) => email.Trim().ToLowerInvariant();

    private static void ValidateEmail(string email)
    {
        if (email.Length is < 5 or > 254 || !email.Contains('@') || !email.Contains('.'))
            throw new ArgumentException("Invalid email format.");
    }

    private static void ValidatePassword(string pwd)
    {
        if (string.IsNullOrWhiteSpace(pwd) || pwd.Length < 8)
            throw new ArgumentException("Password must be at least 8 characters.");
        if (!pwd.Any(char.IsLetter) || !pwd.Any(char.IsDigit))
            throw new ArgumentException("Password must contain letters and digits.");
    }

    private static string Sha256Base64(string value)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(value));
        return Convert.ToBase64String(bytes);
    }
}
