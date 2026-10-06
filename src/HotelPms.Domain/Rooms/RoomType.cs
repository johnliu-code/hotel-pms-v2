using HotelPms.Domain.Properties;

namespace HotelPms.Domain.Rooms;

/// <summary>An accommodation type belonging to one property.</summary>
public sealed class RoomType
{
    public Property Property { get; }
    public string Code { get; }
    public string Name { get; }
    public string? Description { get; }
    public int OccupancyLimit { get; }
    public bool IsActive { get; }

    public RoomType(Property property, string code, string name, int occupancyLimit,
        bool isActive, string? description = null)
    {
        ArgumentNullException.ThrowIfNull(property);
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(occupancyLimit);

        Property = property;
        Code = code;
        Name = name;
        OccupancyLimit = occupancyLimit;
        IsActive = isActive;
        Description = description;
    }
}
