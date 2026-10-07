namespace HotelPms.Domain.Guests;

/// <summary>An actual staying person, distinct from the booking contact.</summary>
public sealed class Guest
{
    public string Name { get; }

    public Guest(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Name = name;
    }
}
