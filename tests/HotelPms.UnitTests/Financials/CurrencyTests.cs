using HotelPms.Domain.Financials;

namespace HotelPms.UnitTests.Financials;

public sealed class CurrencyTests
{
    [Theory]
    [InlineData("CAD", "CAD")]
    [InlineData("usd", "USD")]
    [InlineData("cAd", "CAD")]
    public void NormalizesAsciiCodes(string input, string expected)
    {
        Assert.Equal(expected, new Currency(input).Code);
    }

    [Theory]
    [InlineData("")]
    [InlineData("CA")]
    [InlineData("CADD")]
    [InlineData(" CAD")]
    [InlineData("CAD ")]
    [InlineData("C D")]
    [InlineData("US1")]
    [InlineData("U$D")]
    [InlineData("CÁD")]
    public void RejectsMalformedCodes(string input)
    {
        var exception = Assert.Throws<ArgumentException>(() => new Currency(input));
        Assert.Equal("code", exception.ParamName);
    }

    [Fact]
    public void RejectsNullCode()
    {
        var exception = Assert.Throws<ArgumentNullException>(() => new Currency(null!));
        Assert.Equal("code", exception.ParamName);
    }

    [Fact]
    public void EqualityAndHashingUseNormalizedCode()
    {
        var first = new Currency("cad");
        var second = new Currency("CAD");

        Assert.Equal(first, second);
        Assert.Equal(first.GetHashCode(), second.GetHashCode());
        Assert.NotEqual(first, new Currency("USD"));
        Assert.Single(new HashSet<Currency> { first, second });
    }
}
