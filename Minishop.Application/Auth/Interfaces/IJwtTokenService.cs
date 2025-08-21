using Minishop.Core.Entities;

namespace Minishop.Application.Auth.Interfaces;

public interface IJwtTokenService
{
    string CreateAccessToken(User user, TimeSpan lifetime);
    string CreateRefreshTokenPlaintext(int bytes = 48); // returns plaintext
}