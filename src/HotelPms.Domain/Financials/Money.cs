namespace HotelPms.Domain.Financials;

/// <summary>An exact decimal amount and currency, without rounding or conversion rules.</summary>
public sealed record Money
{
    public decimal Amount { get; }
    public Currency Currency { get; }

    public Money(decimal amount, Currency currency)
    {
        ArgumentNullException.ThrowIfNull(currency);
        Amount = amount;
        Currency = currency;
    }
}
