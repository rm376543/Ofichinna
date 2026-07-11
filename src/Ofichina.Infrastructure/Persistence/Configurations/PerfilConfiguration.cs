using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ofichina.Domain.Entities;

namespace Ofichina.Infrastructure.Persistence.Configurations;

public class PerfilConfiguration : IEntityTypeConfiguration<Perfil>
{
    public void Configure(EntityTypeBuilder<Perfil> builder)
    {
        builder.ToTable("Perfil");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.NomePerfil)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(x => x.NomePerfil)
            .IsUnique();

        builder.HasMany(x => x.Usuarios)
            .WithOne(x => x.Perfil)
            .HasForeignKey(x => x.PerfilId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}