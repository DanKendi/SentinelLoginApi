using FirebaseAdmin;
using FluentValidation;
using Google.Apis.Auth.OAuth2;
using Microsoft.EntityFrameworkCore;
using SentinelApi.Application.Interfaces;
using SentinelApi.Application.UseCases.Auth;
using SentinelApi.Application.UseCases.Usuario;
using SentinelApi.Application.Validators;
using SentinelApi.Domain.Interfaces;
using SentinelApi.Infrastructure.Persistence.Context;
using SentinelApi.Infrastructure.Persistence.Repositories;
using SentinelApi.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Firebase Admin SDK
FirebaseApp.Create(new AppOptions
{
    Credential = GoogleCredential.FromFile(
        Path.Combine(Directory.GetCurrentDirectory(), "serviceAccountKey.json")
    )
});

// DbContext Oracle
builder.Services.AddDbContext<SentinelDbContext>(options =>
    options.UseOracle(builder.Configuration.GetConnectionString("Oracle")));

// Repositórios
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IRegiaoRepository, RegiaoRepository>();

// Firebase Auth Service
builder.Services.AddHttpClient<IFirebaseAuthService, FirebaseAuthService>();

// Use Cases
builder.Services.AddScoped<SentinelApi.Application.UseCases.Auth.RegisterUserUseCase>();
builder.Services.AddScoped<SentinelApi.Application.UseCases.Auth.LoginUserUseCase>();
builder.Services.AddScoped<SentinelApi.Application.UseCases.Usuario.UpdateProfileUseCase>();

// Validators
builder.Services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();

builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();