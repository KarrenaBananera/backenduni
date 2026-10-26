namespace AngularApp3.Server.DTOs.Auth;

public class AuthResponse
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public string Email { get; set; } = string.Empty;
    public IReadOnlyList<string> Roles { get; set; } = [];
}
