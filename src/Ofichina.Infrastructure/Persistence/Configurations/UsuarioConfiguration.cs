using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ofichina.Domain.Entities;
using Ofichina.Domain.ValueObjects;
using Ofichina.Infrastructure.Persistence.Seeds;

namespace Ofichina.Infrastructure.Persistence.Configurations;

public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        var adminEmail = Email.Criar("admin@ofichinna.local");

        builder.ToTable("Usuarios");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Nome)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(x => x.Email)
            .HasConversion(
                email => email.Value,
                value => Email.Criar(value))
            .HasMaxLength(200)
            .IsRequired();

        builder.HasIndex(x => x.Email)
            .IsUnique();

        builder.Property(x => x.SenhaHash)
            .HasMaxLength(500)
            .IsRequired();

        builder.HasMany(x => x.Perfis)
            .WithOne(x => x.Usuario)
            .HasForeignKey(x => x.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasData(new Usuario(
            nome: "Administrador",
            email: adminEmail,
            senhaHash: AuthSeed.AdminPasswordHash)
        {
            Id = AuthSeed.AdminUsuarioId,
            CreatedAt = new DateTime(2026, 7, 7, 21, 43, 51, 914, DateTimeKind.Utc).AddTicks(7864),
            Ativo = true
        });
    }
}