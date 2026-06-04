using Microsoft.EntityFrameworkCore;
using SentinelApi.Infrastructure.Persistence.Context;

var builder = WebApplication.CreateBuilder(args);

// DbContext Oracle
builder.Services.AddDbContext<SentinelDbContext>(options =>
    options.UseOracle(builder.Configuration.GetConnectionString("Oracle")));

builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();