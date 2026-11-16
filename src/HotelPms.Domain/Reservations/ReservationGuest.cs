using HotelPms.Domain.Guests;

namespace HotelPms.Domain.Reservations;

/// <summary>A guest's designation within a reservation context; no reservation-wide rules are enforced here.</summary>
public sealed class ReservationGuest
{
    public Guid Id { get; private set; }
    public Guest Guest { get; }
    public bool IsPrimary { get; }

    public ReservationGuest(Guest guest, bool isPrimary)
    {
        ArgumentNullException.ThrowIfNull(guest);

        Id = Guid.NewGuid();
        Guest = guest;
        IsPrimary = isPrimary;
    }
}
