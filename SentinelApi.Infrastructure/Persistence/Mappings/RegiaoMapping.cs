namespace SentinelApi.Infrastructure.Persistence.Mappings;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SentinelApi.Domain.Entities;

public class RegiaoMapping : IEntityTypeConfiguration<Regiao>
{
    public void Configure(EntityTypeBuilder<Regiao> builder)
    {
        builder.ToTable("T_SEN_REGIAO");

        builder.HasKey(r => r.IdRegiao);

        builder.Property(r => r.IdRegiao)
            .HasColumnName("ID_REGIAO")
            .ValueGeneratedOnAdd();

        builder.Property(r => r.NmRegiao)
            .HasColumnName("NM_REGIAO")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(r => r.NmEstado)
            .HasColumnName("NM_ESTADO")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(r => r.NmPais)
            .HasColumnName("NM_PAIS")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(r => r.ReLatitude)
            .HasColumnName("RE_LATITUDE")
            .HasPrecision(10, 6)
            .IsRequired();

        builder.Property(r => r.ReLongitude)
            .HasColumnName("RE_LONGITUDE")
            .HasPrecision(10, 6)
            .IsRequired();

        // Relacionamento N:N com Usuario via UsuarioRegiao
        builder.HasMany(r => r.UsuarioRegioes)
            .WithOne(ur => ur.Regiao)
            .HasForeignKey(ur => ur.IdRegiao);
    }
}