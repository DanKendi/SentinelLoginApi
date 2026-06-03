namespace SentinelApi.Application.Interfaces;

public interface IFirebaseAuthService
{
    Task<string> CreateUserAsync(string email, string password, string displayName);
    Task<(string IdToken, string Uid)> SignInAsync(string email, string password);
}