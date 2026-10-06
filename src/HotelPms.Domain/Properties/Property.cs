using HotelPms.Domain.Financials;

namespace HotelPms.Domain.Properties;

/// <summary>A property with minimal address/contact details and a timezone identifier.</summary>
public sealed class Property
{
    public string Name { get; }
    public string TimeZoneId { get; }
    public Currency Currency { get; }
    public bool IsActive { get; }
    public string? AddressLine1 { get; }
    public string? AddressLine2 { get; }
    public string? City { get; }
    public string? Region { get; }
    public string? PostalCode { get; }
    public string? CountryCode { get; }
    public string? Phone { get; }
    public string? Email { get; }

    public Property(string name, string timeZoneId, Currency currency, bool isActive,
        string? addressLine1 = null, string? addressLine2 = null,
        string? city = null, string? region = null, string? postalCode = null,
        string? countryCode = null, string? phone = null, string? email = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(timeZoneId);
        ArgumentNullException.ThrowIfNull(currency);

        Name = name;
        TimeZoneId = timeZoneId;
        Currency = currency;
        IsActive = isActive;
        AddressLine1 = addressLine1;
        AddressLine2 = addressLine2;
        City = city;
        Region = region;
        PostalCode = postalCode;
        CountryCode = countryCode;
        Phone = phone;
        Email = email;
    }
}
