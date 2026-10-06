using HotelPms.Domain.Financials;
using HotelPms.Domain.Properties;

namespace HotelPms.UnitTests.Properties;

public sealed class PropertyTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void PreservesNameCurrencyAndActiveState(bool active)
    {
        var currency = new Currency("cad");
        var property = new Property("Cabin Retreat", "America/Vancouver", currency, active);
        Assert.Equal("Cabin Retreat", property.Name);
        Assert.Equal("America/Vancouver", property.TimeZoneId);
        Assert.Same(currency, property.Currency);
        Assert.Equal(active, property.IsActive);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" \t")]
    public void RejectsMissingName(string? name)
    {
        var error = Assert.ThrowsAny<ArgumentException>(() => new Property(name!, "America/Vancouver", new Currency("CAD"), true));
        Assert.Equal("name", error.ParamName);
    }

    [Fact]
    public void RejectsMissingCurrency()
    {
        Assert.Equal("currency", Assert.Throws<ArgumentNullException>(() => new Property("Hotel", "America/Vancouver", null!, true)).ParamName);
    }

    [Fact]
    public void MatchingFieldsDoNotMakeDistinctPropertiesTheSameEntity()
    {
        Assert.NotEqual(new Property("Hotel", "America/Vancouver", new Currency("CAD"), true),
            new Property("Hotel", "America/Vancouver", new Currency("CAD"), true));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" \t")]
    public void RejectsMissingTimeZoneId(string? timeZoneId)
    {
        var error = Assert.ThrowsAny<ArgumentException>(() =>
            new Property("Hotel", timeZoneId!, new Currency("CAD"), true));
        Assert.Equal("timeZoneId", error.ParamName);
    }

    [Theory]
    [InlineData("America/Vancouver")]
    [InlineData("Unregistered/Zone")]
    [InlineData(" Custom identifier ")]
    public void PreservesNonBlankTimeZoneIdWithoutRegistryValidation(string timeZoneId)
    {
        var property = new Property("Hotel", timeZoneId, new Currency("CAD"), true);
        Assert.Equal(timeZoneId, property.TimeZoneId);
    }

    [Fact]
    public void AddressAndContactFieldsDefaultToNull()
    {
        var property = new Property("Hotel", "UTC", new Currency("CAD"), true);
        Assert.Null(property.AddressLine1);
        Assert.Null(property.AddressLine2);
        Assert.Null(property.City);
        Assert.Null(property.Region);
        Assert.Null(property.PostalCode);
        Assert.Null(property.CountryCode);
        Assert.Null(property.Phone);
        Assert.Null(property.Email);
    }

    [Theory]
    [InlineData("1 Cabin Road", "Unit A", "Whistler", "BC", "V0N 1B0", "CA", "+1 555 0100", "frontdesk@example.test")]
    [InlineData("", " ", "", " ", "", "unregistered", "free-form phone", "free-form email")]
    public void PreservesOptionalAddressAndContactScalarsWithoutFormatRules(
        string addressLine1, string addressLine2, string city, string region,
        string postalCode, string countryCode, string phone, string email)
    {
        var property = new Property("Hotel", "UTC", new Currency("CAD"), true,
            addressLine1, addressLine2, city, region, postalCode, countryCode, phone, email);
        Assert.Equal(addressLine1, property.AddressLine1);
        Assert.Equal(addressLine2, property.AddressLine2);
        Assert.Equal(city, property.City);
        Assert.Equal(region, property.Region);
        Assert.Equal(postalCode, property.PostalCode);
        Assert.Equal(countryCode, property.CountryCode);
        Assert.Equal(phone, property.Phone);
        Assert.Equal(email, property.Email);
    }
}
