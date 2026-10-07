using HotelPms.Domain.Customers;
using HotelPms.Domain.Guests;

namespace HotelPms.UnitTests.Customers;

public sealed class CustomerTests
{
    [Theory]
    [InlineData("Alex")]
    [InlineData("李 明")]
    [InlineData(" Cabin Company ")]
    public void PreservesNameWithoutSplittingOrNormalization(string name)
    {
        Assert.Equal(name, new Customer(name).Name);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" \t")]
    public void RejectsMissingName(string? name)
    {
        Assert.Equal("name", Assert.ThrowsAny<ArgumentException>(() => new Customer(name!)).ParamName);
    }

    [Fact]
    public void ContactFieldsAreOptional()
    {
        var customer = new Customer("Alex");
        Assert.Null(customer.Phone);
        Assert.Null(customer.Email);
    }

    [Theory]
    [InlineData("+1 555 0100", "contact@example.test")]
    [InlineData("", " ")]
    [InlineData("free-form phone", "free-form email")]
    public void PreservesContactScalarsWithoutFormatRules(string phone, string email)
    {
        var customer = new Customer("Alex", phone, email);
        Assert.Equal(phone, customer.Phone);
        Assert.Equal(email, customer.Email);
    }

    [Fact]
    public void MatchingNamesDoNotMergeDistinctCustomers()
    {
        var first = new Customer("Alex");
        var second = new Customer("Alex");
        Assert.NotEqual(first, second);
        Assert.Equal(2, new HashSet<Customer> { first, second, first }.Count);
    }

    [Fact]
    public void BookingContactAndStayingPersonRemainSeparate()
    {
        var customer = new Customer("Alex", email: "contact@example.test");
        var guest = new Guest("Alex");
        Assert.NotEqual<object>(customer, guest);
        Assert.Equal(customer.Name, guest.Name);
    }

    [Fact]
    public void AddressFieldsDefaultToNull()
    {
        var customer = new Customer("Alex");
        Assert.Null(customer.AddressLine1);
        Assert.Null(customer.AddressLine2);
        Assert.Null(customer.City);
        Assert.Null(customer.Region);
        Assert.Null(customer.PostalCode);
        Assert.Null(customer.CountryCode);
    }

    [Theory]
    [InlineData("1 Cabin Road", "Unit A", "Whistler", "BC", "V0N 1B0", "CA")]
    [InlineData("", " ", "\t", " free-form region ", "not-a-postal-code", "unregistered")]
    [InlineData(null, null, null, null, null, null)]
    [InlineData("一丁目", null, "東京", null, null, "JP")]
    public void PreservesOptionalAddressScalarsWithoutFormatRules(
        string? addressLine1, string? addressLine2, string? city,
        string? region, string? postalCode, string? countryCode)
    {
        var customer = new Customer("Alex", " phone as supplied ", " email as supplied ",
            addressLine1, addressLine2, city, region, postalCode, countryCode);
        Assert.Equal(addressLine1, customer.AddressLine1);
        Assert.Equal(addressLine2, customer.AddressLine2);
        Assert.Equal(city, customer.City);
        Assert.Equal(region, customer.Region);
        Assert.Equal(postalCode, customer.PostalCode);
        Assert.Equal(countryCode, customer.CountryCode);
        Assert.Equal(" phone as supplied ", customer.Phone);
        Assert.Equal(" email as supplied ", customer.Email);
    }
}
