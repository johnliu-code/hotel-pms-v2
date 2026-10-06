namespace HotelPms.Domain.Rooms;

/// <summary>Physical room operation; occupancy is derived from reservation assignments.</summary>
public enum RoomOperationalStatus
{
    Ready = 0,
    OutOfService = 1
}
