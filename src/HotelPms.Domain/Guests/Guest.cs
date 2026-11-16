namespace HotelPms.Domain.Guests;

/// <summary>An actual staying person, distinct from the booking contact.</summary>
public sealed class Guest
{
    public Guid Id { get; private set; }
    public string Name { get; }

    public Guest(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Id = Guid.NewGuid();
        Name = name;
    }
}
