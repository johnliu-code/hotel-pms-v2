using HotelPms.Domain.Properties;

namespace HotelPms.Domain.Rooms;

/// <summary>A physical accommodation unit; operational status does not describe occupancy.</summary>
public sealed class Room
{
    public Property Property { get; }
    public RoomType RoomType { get; }
    public string Code { get; }
    public string? Floor { get; }
    public RoomOperationalStatus OperationalStatus { get; }
    public bool IsActive { get; }
    public string? Notes { get; }

    public Room(Property property, RoomType roomType, string code,
        RoomOperationalStatus operationalStatus, bool isActive,
        string? floor = null, string? notes = null)
    {
        ArgumentNullException.ThrowIfNull(property);
        ArgumentNullException.ThrowIfNull(roomType);
        ArgumentException.ThrowIfNullOrWhiteSpace(code);

        if (!ReferenceEquals(property, roomType.Property))
        {
            throw new ArgumentException("Room type must belong to the room's property.", nameof(roomType));
        }

        if (!Enum.IsDefined(operationalStatus))
        {
            throw new ArgumentOutOfRangeException(nameof(operationalStatus), "Unknown room operational status.");
        }

        Property = property;
        RoomType = roomType;
        Code = code;
        OperationalStatus = operationalStatus;
        IsActive = isActive;
        Floor = floor;
        Notes = notes;
    }
}
