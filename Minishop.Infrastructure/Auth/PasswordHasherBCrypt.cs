using BCrypt.Net;
using Minishop.Application.Auth.Interfaces;

namespace Minishop.Infrastructure.Auth;

public sealed class PasswordHasherBCrypt : IPasswordHasher
{
    private const int WorkFactor = 12;
    public string Hash(string password) => BCrypt.Net.BCrypt.HashPassword(password, WorkFactor);
    public bool Verify(string password, string passwordHash) => BCrypt.Net.BCrypt.Verify(password, passwordHash);
}