using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ofichina.Domain.Entities;
using Ofichina.Domain.ValueObjects;

namespace Ofichina.Infrastructure.Persistence.Configurations;

public class PessoaConfiguration : IEntityTypeConfiguration<Pessoa>
{
    public void Configure(EntityTypeBuilder<Pessoa> builder)
    {
        builder.ToTable("Pessoas");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Nome)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.UsuarioId)
            .IsRequired();

        builder.Navigation(c => c.Veiculos)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Property(c => c.Documento)
            .HasConversion(
                documento => documento.Numero,
                numero => CriarDocumento(numero))
            .HasMaxLength(14)
            .IsRequired();

        builder.Property(c => c.Telefone)
            .HasConversion(
                telefone => telefone.Value,
                valor => Telefone.Criar(valor))
            .HasMaxLength(11)
            .IsRequired();

        builder.Property(c => c.Email)
            .HasConversion(
                email => email.Value,
                valor => new Email(valor))
            .HasMaxLength(200)
            .IsRequired();

        builder.OwnsOne(c => c.Endereco, endereco =>
        {
            endereco.Property(e => e.Logradouro)
                .HasColumnName("EnderecoLogradouro")
                .HasMaxLength(200)
                .IsRequired();

            endereco.Property(e => e.Numero)
                .HasColumnName("EnderecoNumero")
                .HasMaxLength(20)
                .IsRequired();

            endereco.Property(e => e.Complemento)
                .HasColumnName("EnderecoComplemento")
                .HasMaxLength(100)
                .IsRequired();

            endereco.Property(e => e.Bairro)
                .HasColumnName("EnderecoBairro")
                .HasMaxLength(100)
                .IsRequired();

            endereco.Property(e => e.Cidade)
                .HasColumnName("EnderecoCidade")
                .HasMaxLength(100)
                .IsRequired();

            endereco.Property(e => e.Estado)
                .HasColumnName("EnderecoEstado")
                .HasMaxLength(2)
                .IsRequired();

            endereco.Property(e => e.Cep)
                .HasConversion(
                    cep => cep.Value,
                    valor => Cep.Criar(valor))
                .HasColumnName("EnderecoCep")
                .HasMaxLength(8)
                .IsRequired();
        });

        builder.HasOne(c => c.Usuario)
            .WithOne()
            .HasForeignKey<Pessoa>(c => c.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    private static Documento CriarDocumento(string numero)
    {
        return numero.Length switch
        {
            11 => new Cpf(numero),
            14 => new Cnpj(numero),
            _ => throw new InvalidOperationException("Documento inválido.")
        };
    }
}
