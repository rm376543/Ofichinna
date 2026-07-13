using System.Net.Mail;
using Ofichina.Domain.Exceptions;

namespace Ofichina.Domain.ValueObjects;

/// <summary>
/// Representa um endereço de e-mail do domínio.
/// </summary>
public sealed class Email : ValueObject
{
    public string Value { get; }

    private Email(string value)
    {
        Value = value;
    }

    public static Email Criar(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("E-mail inválido.");

        try
        {
            var address = new MailAddress(email.Trim());
            return new Email(address.Address.Trim().ToLowerInvariant());
        }
        catch (FormatException ex)
        {
            throw new DomainException("E-mail inválido.", ex);
        }
    }

    public override string ToString()
    {
        return Value;
    }

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}