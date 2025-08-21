namespace Minishop.Application.Auth.DTOs;
public sealed class RefreshRequest
{
    public string RefreshToken { get; set; } = null!;
}