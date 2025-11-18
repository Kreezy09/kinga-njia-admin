public interface IAuthService
{
    Task<string?> RegisterAsync(RegisterRequestDto registerRequest);
    Task<string?> LoginAsync(LoginRequestDto loginRequest);
}