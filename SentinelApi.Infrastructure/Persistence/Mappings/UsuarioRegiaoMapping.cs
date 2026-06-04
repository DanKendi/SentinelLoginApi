namespace SentinelApi.Infrastructure.Persistence.Mappings;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SentinelApi.Domain.Entities;

public class UsuarioRegiaoMapping : IEntityTypeConfiguration<UsuarioRegiao>
{
    public void Configure(EntityTypeBuilder<UsuarioRegiao> builder)
    {
        builder.ToTable("T_SEN_USUARIO_REGIAO");

        // Chave composta (PK da tabela associativa)
        builder.HasKey(ur => new { ur.IdUsuario, ur.IdRegiao });

        builder.Property(ur => ur.IdUsuario)
            .HasColumnName("T_SEN_USUARIO_ID_USUARIO");

        builder.Property(ur => ur.IdRegiao)
            .HasColumnName("T_SEN_REGIAO_ID_REGIAO");

        builder.Property(ur => ur.DataInscricao)
            .HasColumnName("DATA_INSCRICAO")
            .IsRequired();

        builder.Property(ur => ur.Ativo)
            .HasColumnName("US_RE_ATIVO")
            .HasMaxLength(1)
            .IsRequired();
    }
}