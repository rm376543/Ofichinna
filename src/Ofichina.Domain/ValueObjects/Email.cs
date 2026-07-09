using System.Net.Mail;

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
        ArgumentException.ThrowIfNullOrWhiteSpace(email);

        try
        {
            var address = new MailAddress(email.Trim());
            return new Email(address.Address.Trim().ToLowerInvariant());
        }
        catch (FormatException ex)
        {
            throw new ArgumentException("E-mail inválido.", nameof(email), ex);
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