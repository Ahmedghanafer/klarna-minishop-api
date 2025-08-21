namespace Minishop.Application.Auth.DTOs;
public sealed class LoginResponse
{
    public string AccessToken { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
}
