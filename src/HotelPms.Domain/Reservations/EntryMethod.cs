namespace HotelPms.Domain.Reservations;

/// <summary>How a reservation was entered, independently of its booking channel.</summary>
public enum EntryMethod
{
    Manual = 0,
    Import = 1,
    Integration = 2
}
