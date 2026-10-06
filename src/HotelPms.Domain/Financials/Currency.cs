namespace HotelPms.Domain.Financials;

/// <summary>A normalized three-letter currency code; not a registry of supported currencies.</summary>
public sealed record Currency
{
    public string Code { get; }

    public Currency(string code)
    {
        ArgumentNullException.ThrowIfNull(code);

        if (code.Length != 3 || !code.All(char.IsAsciiLetter))
        {
            throw new ArgumentException("Currency code must contain exactly three ASCII letters.", nameof(code));
        }

        Code = code.ToUpperInvariant();
    }
}
