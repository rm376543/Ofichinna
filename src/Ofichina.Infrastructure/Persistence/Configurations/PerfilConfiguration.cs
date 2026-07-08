using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ofichina.Domain.Entities;
using Ofichina.Infrastructure.Persistence.Seeds;

namespace Ofichina.Infrastructure.Persistence.Configurations;

public class PerfilConfiguration : IEntityTypeConfiguration<Perfil>
{
    public void Configure(EntityTypeBuilder<Perfil> builder)
    {
        builder.ToTable("Perfis");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Codigo)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(x => x.Codigo)
            .IsUnique();

        builder.Property(x => x.Nome)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(x => x.Descricao)
            .HasMaxLength(300);

        builder.HasMany(x => x.Usuarios)
            .WithOne(x => x.Perfil)
            .HasForeignKey(x => x.PerfilId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasData(new Perfil(
            codigo: "ADMIN",
            nome: "Administrador",
            descricao: "Perfil com acesso total ao sistema")
        {
            Id = AuthSeed.AdminPerfilId,
            CreatedAt = new DateTime(2026, 7, 7, 21, 43, 51, 903, DateTimeKind.Utc).AddTicks(6297),
            Ativo = true
        });
    }
}