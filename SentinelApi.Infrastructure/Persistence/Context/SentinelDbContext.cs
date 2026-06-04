namespace SentinelApi.Infrastructure.Persistence.Context;

using Microsoft.EntityFrameworkCore;
using SentinelApi.Domain.Entities;
using SentinelApi.Infrastructure.Persistence.Mappings;

public class SentinelDbContext : DbContext
{
    public SentinelDbContext(DbContextOptions<SentinelDbContext> options)
        : base(options) { }

    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Regiao> Regioes { get; set; }
    public DbSet<UsuarioRegiao> UsuarioRegioes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UsuarioMapping());
        modelBuilder.ApplyConfiguration(new RegiaoMapping());
        modelBuilder.ApplyConfiguration(new UsuarioRegiaoMapping());

        base.OnModelCreating(modelBuilder);
    }
}