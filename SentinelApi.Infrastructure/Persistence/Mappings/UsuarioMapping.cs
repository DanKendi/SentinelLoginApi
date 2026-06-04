namespace SentinelApi.Infrastructure.Persistence.Mappings;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SentinelApi.Domain.Entities;

public class UsuarioMapping : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("T_SEN_USUARIO");

        builder.HasKey(u => u.IdUsuario);

        builder.Property(u => u.IdUsuario)
            .HasColumnName("ID_USUARIO")
            .ValueGeneratedOnAdd();

        builder.Property(u => u.Nome)
            .HasColumnName("NOME")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(u => u.Email)
            .HasColumnName("EMAIL")
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(u => u.SenhaHash)
            .HasColumnName("SENHA_HASH")
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(u => u.FcmToken)
            .HasColumnName("FCM_TOKEN")
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(u => u.UidFirebase)
            .HasColumnName("UID_FIREBASE")
            .HasMaxLength(128);

        builder.Property(u => u.Latitude)
            .HasColumnName("LATITUDE")
            .HasPrecision(10, 6);

        builder.Property(u => u.Longitude)
            .HasColumnName("LONGITUDE")
            .HasPrecision(10, 6);

        builder.Property(u => u.RaioKm)
            .HasColumnName("RAIO_KM")
            .IsRequired();

        builder.Property(u => u.DataCadastro)
            .HasColumnName("DATA_CADASTRO")
            .IsRequired();

        builder.Property(u => u.Ativo)
            .HasColumnName("ATIVO")
            .HasMaxLength(1)
            .IsRequired();

        // Relacionamento N:N com Regiao via UsuarioRegiao
        builder.HasMany(u => u.UsuarioRegioes)
            .WithOne(ur => ur.Usuario)
            .HasForeignKey(ur => ur.IdUsuario);
    }
}