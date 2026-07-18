using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ofichina.Domain.Entities;
using Ofichina.Domain.ValueObjects;

namespace Ofichina.Infrastructure.Persistence.Configurations;

public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("Usuarios");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Email)
            .HasConversion(
                email => email.Value,
                value => new Email(value))
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.SenhaHash)
            .HasMaxLength(500)
            .IsRequired();

        builder.HasMany(x => x.Perfis)
            .WithOne(x => x.Usuario)
            .HasForeignKey(x => x.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.Perfis)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}