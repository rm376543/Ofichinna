namespace Ofichina.Domain.ValueObjects
{
    public enum TipoDocumento
    {
        CPF = 1,
        CNPJ = 2
    }

    public abstract class Documento : ValueObject
    {
        public string Numero { get; protected set; } = string.Empty;

        public abstract TipoDocumento Tipo { get; }

        protected Documento()
        {
        }

        protected Documento(string numero)
        {
            Numero = numero;
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Numero;
            yield return Tipo;
        }

        public override string ToString() => Numero;
    }
}