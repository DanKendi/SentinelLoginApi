namespace SentinelApi.Infrastructure.Services;

using FirebaseAdmin.Auth;
using Microsoft.Extensions.Configuration;
using SentinelApi.Application.Interfaces;
using SentinelApi.Domain.Exceptions;
using System.Text;
using System.Text.Json;

public class FirebaseAuthService : IFirebaseAuthService
{
    private readonly HttpClient _httpClient;
    private readonly string _webApiKey;

    public FirebaseAuthService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _webApiKey = configuration["Firebase:WebApiKey"]!;
    }

    public async Task<string> CreateUserAsync(string email, string password, string displayName)
    {
        try
        {
            var userArgs = new UserRecordArgs
            {
                Email = email,
                Password = password,
                DisplayName = displayName,
                EmailVerified = false,
                Disabled = false
            };

            var userRecord = await FirebaseAuth.DefaultInstance.CreateUserAsync(userArgs);
            return userRecord.Uid;
        }
        catch (FirebaseAuthException ex)
        {
            // E-mail já existe no Firebase
            if (ex.AuthErrorCode == AuthErrorCode.EmailAlreadyExists)
                throw new DomainException("E-mail já cadastrado.");

            throw new DomainException($"Erro ao criar usuário no Firebase: {ex.Message}");
        }
    }

    public async Task<(string IdToken, string Uid)> SignInAsync(string email, string password)
    {
        // O Admin SDK não expõe login por senha — usamos a REST API do Firebase
        var url = $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={_webApiKey}";

        var payload = new
        {
            email,
            password,
            returnSecureToken = true
        };

        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(url, content);

        if (!response.IsSuccessStatusCode)
            throw new DomainException("E-mail ou senha inválidos.");

        var responseBody = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<JsonElement>(responseBody);

        var idToken = result.GetProperty("idToken").GetString()!;
        var uid = result.GetProperty("localId").GetString()!;

        return (idToken, uid);
    }
}