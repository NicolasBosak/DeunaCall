namespace DeunaCall.Shared.DTOs;

public class LoginDto
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class AuthResponseDto
{
    public string Token { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}

public class PatientLoginDto
{
    public string AccessCode { get; set; } = string.Empty;
}
