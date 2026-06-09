# ---- Build Stage ----
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
 
# Copia os arquivos de projeto e restaura dependências
COPY ["SentinelApi.WebApi/SentinelApi.WebApi.csproj", "SentinelApi.WebApi/"]
COPY ["SentinelApi.Application/SentinelApi.Application.csproj", "SentinelApi.Application/"]
COPY ["SentinelApi.Infrastructure/SentinelApi.Infrastructure.csproj", "SentinelApi.Infrastructure/"]
COPY ["SentinelApi.Domain/SentinelApi.Domain.csproj", "SentinelApi.Domain/"]
 
RUN dotnet restore "SentinelApi.WebApi/SentinelApi.WebApi.csproj"
 
# Copia o restante do código e publica
COPY . .
WORKDIR "/src/SentinelApi.WebApi"
RUN dotnet publish "SentinelApi.WebApi.csproj" -c Release -o /app/publish --no-restore
 
# ---- Runtime Stage ----
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
 
COPY --from=build /app/publish .
 
# Railway injeta PORT automaticamente
ENV ASPNETCORE_URLS=http://0.0.0.0:8080
ENV ASPNETCORE_ENVIRONMENT=Production
 
EXPOSE 8080
 
ENTRYPOINT ["dotnet", "SentinelApi.WebApi.dll"]