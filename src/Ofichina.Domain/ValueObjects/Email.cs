using System.Net.Mail;
using Ofichina.Domain.Exceptions;

namespace Ofichina.Domain.ValueObjects;

/// <summary>
/// Representa um endereço de e-mail válido.
/// </summary>
public sealed class Email : ValueObject
{
    public string Value { get; private set; } = null!;

    private Email()
    {
    }

    public Email(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("E-mail inválido.");

        try
        {
            var address = new MailAddress(email.Trim());

            Value = address.Address.Trim().ToLowerInvariant();
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