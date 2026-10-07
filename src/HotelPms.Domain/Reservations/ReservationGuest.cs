using HotelPms.Domain.Guests;

namespace HotelPms.Domain.Reservations;

/// <summary>A guest's designation within a reservation context; no reservation-wide rules are enforced here.</summary>
public sealed class ReservationGuest
{
    public Guest Guest { get; }
    public bool IsPrimary { get; }

    public ReservationGuest(Guest guest, bool isPrimary)
    {
        ArgumentNullException.ThrowIfNull(guest);
        Guest = guest;
        IsPrimary = isPrimary;
    }
}
