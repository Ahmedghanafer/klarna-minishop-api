namespace Minishop.Application.Auth.DTOs;
public sealed class LoginRequest
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}