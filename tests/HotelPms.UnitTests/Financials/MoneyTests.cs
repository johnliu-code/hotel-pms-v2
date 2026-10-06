using HotelPms.Domain.Financials;

namespace HotelPms.UnitTests.Financials;

public sealed class MoneyTests
{
    [Fact]
    public void PreservesAmountWithoutRoundingOrSignRestrictions()
    {
        var currency = new Currency("CAD");
        decimal[] amounts = [0m, -12.3456m, 12.3456m, decimal.MinValue, decimal.MaxValue];

        foreach (var amount in amounts)
        {
            var money = new Money(amount, currency);
            Assert.Equal(amount, money.Amount);
            Assert.Equal(currency, money.Currency);
        }
    }

    [Fact]
    public void RejectsNullCurrency()
    {
        var exception = Assert.Throws<ArgumentNullException>(() => new Money(10m, null!));
        Assert.Equal("currency", exception.ParamName);
    }

    [Fact]
    public void EqualityAndHashingUseAmountAndCurrency()
    {
        var first = new Money(10.0m, new Currency("cad"));
        var second = new Money(10.00m, new Currency("CAD"));

        Assert.Equal(first, second);
        Assert.Equal(first.GetHashCode(), second.GetHashCode());
        Assert.NotEqual(first, new Money(11m, new Currency("CAD")));
        Assert.NotEqual(first, new Money(10m, new Currency("USD")));
        Assert.Single(new HashSet<Money> { first, second });
    }
}
