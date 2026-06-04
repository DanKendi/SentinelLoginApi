using FirebaseAdmin;
using FluentValidation;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SentinelApi.Application.Interfaces;
using SentinelApi.Application.UseCases.Auth;
using SentinelApi.Application.UseCases.Usuario;
using SentinelApi.Application.Validators;
using SentinelApi.Domain.Interfaces;
using SentinelApi.Infrastructure.Persistence.Context;
using SentinelApi.Infrastructure.Persistence.Repositories;
using SentinelApi.Infrastructure.Services;
using SentinelApi.WebApi.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Firebase Admin SDK
FirebaseApp.Create(new AppOptions
{
    Credential = GoogleCredential.FromFile(
        Path.Combine(Directory.GetCurrentDirectory(), "serviceAccountKey.json"))
});

// DbContext Oracle
builder.Services.AddDbContext<SentinelDbContext>(options =>
    options.UseOracle(builder.Configuration.GetConnectionString("Oracle")));

// Repositorios
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IRegiaoRepository, RegiaoRepository>();

// Firebase Auth Service
builder.Services.AddHttpClient<IFirebaseAuthService, FirebaseAuthService>();

// Use Cases
builder.Services.AddScoped<RegisterUserUseCase>();
builder.Services.AddScoped<LoginUserUseCase>();
builder.Services.AddScoped<UpdateProfileUseCase>();

// Validators
builder.Services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();

// Autenticacao JWT (Firebase)
var firebaseProjectId = builder.Configuration["Firebase:ProjectId"]!;

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = $"https://securetoken.google.com/{firebaseProjectId}";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = $"https://securetoken.google.com/{firebaseProjectId}",
            ValidateAudience = true,
            ValidAudience = firebaseProjectId,
            ValidateLifetime = true
        };
    });

builder.Services.AddAuthorization();

// Health Checks
builder.Services.AddHealthChecks()
    .AddOracle(
        builder.Configuration.GetConnectionString("Oracle")!,
        name: "oracle-db",
        tags: new[] { "db", "oracle" });

//Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Sentinel API",
        Version = "v1",
        Description = "API de autenticação do sistema de previsão de desastres naturais"
    });

    // Permite enviar o token JWT pelo Swagger UI
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Informe o idToken retornado pelo login. Ex: Bearer eyJhbGci..."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddControllers();

// Build
var app = builder.Build();

// Pipeline
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Sentinel API v1");
    options.RoutePrefix = string.Empty; // Swagger na raiz: http://localhost:{porta}/
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Health Check endpoint
app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var result = System.Text.Json.JsonSerializer.Serialize(new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                description = e.Value.Description
            })
        });
        await context.Response.WriteAsync(result);
    }
});

app.Run();