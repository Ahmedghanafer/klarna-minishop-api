namespace Minishop.Application.Auth.DTOs;
public sealed class RegisterRequest
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}