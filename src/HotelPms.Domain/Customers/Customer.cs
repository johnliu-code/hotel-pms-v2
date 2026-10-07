namespace HotelPms.Domain.Customers;

/// <summary>The booking contact or payer profile, distinct from an actual staying guest.</summary>
public sealed class Customer
{
    public string Name { get; }
    public string? Phone { get; }
    public string? Email { get; }
    public string? AddressLine1 { get; }
    public string? AddressLine2 { get; }
    public string? City { get; }
    public string? Region { get; }
    public string? PostalCode { get; }
    public string? CountryCode { get; }

    public Customer(string name, string? phone = null, string? email = null,
        string? addressLine1 = null, string? addressLine2 = null,
        string? city = null, string? region = null,
        string? postalCode = null, string? countryCode = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Name = name;
        Phone = phone;
        Email = email;
        AddressLine1 = addressLine1;
        AddressLine2 = addressLine2;
        City = city;
        Region = region;
        PostalCode = postalCode;
        CountryCode = countryCode;
    }
}
