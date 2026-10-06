namespace HotelPms.Domain.Reservations;

/// <summary>The reservation lifecycle, independent of physical room operation.</summary>
public enum ReservationStatus
{
    Draft = 0,
    Confirmed = 1,
    CheckedIn = 2,
    CheckedOut = 3,
    Cancelled = 4
}
