using HotelPms.Domain.Customers;
using HotelPms.Domain.Properties;
using HotelPms.Domain.Rooms;

namespace HotelPms.Domain.Reservations;

/// <summary>A room-type booking with an exclusive checkout date and guarded lifecycle.</summary>
public sealed class Reservation
{
    private readonly List<ReservationGuest> _guests = [];

    public Property Property { get; }
    public RoomType RoomType { get; }
    public Customer Customer { get; }
    public DateOnly CheckInDate { get; }
    /// <summary>The exclusive end of the stay.</summary>
    public DateOnly CheckOutDate { get; }
    public int Adults { get; }
    public int Children { get; }
    public EntryMethod EntryMethod { get; }
    public string? BookingChannelCode { get; }
    public string? Notes { get; }
    public IReadOnlyList<ReservationGuest> Guests { get; }
    public ReservationStatus Status { get; private set; } = ReservationStatus.Draft;
    public string? ConfirmationNumber { get; private set; }

    public Reservation(Property property, RoomType roomType, Customer customer,
        DateOnly checkInDate, DateOnly checkOutDate, int adults, int children,
        EntryMethod entryMethod, IEnumerable<ReservationGuest>? guests = null,
        string? bookingChannelCode = null, string? notes = null)
    {
        ArgumentNullException.ThrowIfNull(property);
        ArgumentNullException.ThrowIfNull(roomType);
        ArgumentNullException.ThrowIfNull(customer);
        if (!ReferenceEquals(property, roomType.Property))
        {
            throw new ArgumentException("Room type must belong to the reservation's property.", nameof(roomType));
        }
        if (checkOutDate <= checkInDate)
        {
            throw new ArgumentException("Checkout must be later than check-in.", nameof(checkOutDate));
        }
        ArgumentOutOfRangeException.ThrowIfLessThan(adults, 1);
        ArgumentOutOfRangeException.ThrowIfNegative(children);
        if ((long)adults + children > roomType.OccupancyLimit)
        {
            throw new ArgumentException("Occupancy exceeds the room type limit.", nameof(adults));
        }
        if (!Enum.IsDefined(entryMethod))
        {
            throw new ArgumentOutOfRangeException(nameof(entryMethod), "Unknown entry method.");
        }

        Property = property;
        RoomType = roomType;
        Customer = customer;
        CheckInDate = checkInDate;
        CheckOutDate = checkOutDate;
        Adults = adults;
        Children = children;
        EntryMethod = entryMethod;
        BookingChannelCode = bookingChannelCode;
        Notes = notes;
        Guests = _guests.AsReadOnly();
        if (guests is not null)
        {
            foreach (var guest in guests)
            {
                AddGuest(guest);
            }
        }
    }

    public void AddGuest(ReservationGuest guest)
    {
        ArgumentNullException.ThrowIfNull(guest);
        if (guest.IsPrimary && _guests.Any(existing => existing.IsPrimary))
        {
            throw new ArgumentException("A reservation may have at most one primary guest.", nameof(guest));
        }
        _guests.Add(guest);
    }

    /// <summary>Applies only an approved transition; confirmation input is accepted only on confirmation.</summary>
    public void TransitionTo(ReservationStatus status, string? confirmationNumber = null)
    {
        var allowed = (Status, status) switch
        {
            (ReservationStatus.Draft, ReservationStatus.Confirmed or ReservationStatus.Cancelled) => true,
            (ReservationStatus.Confirmed, ReservationStatus.CheckedIn or ReservationStatus.Cancelled) => true,
            (ReservationStatus.CheckedIn, ReservationStatus.CheckedOut) => true,
            _ => false
        };
        if (!allowed)
        {
            throw new InvalidOperationException("Reservation lifecycle transition is not allowed.");
        }
        if (status == ReservationStatus.Confirmed)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(confirmationNumber);
            ConfirmationNumber = confirmationNumber;
        }
        else if (confirmationNumber is not null)
        {
            throw new ArgumentException("Confirmation input is accepted only when confirming.", nameof(confirmationNumber));
        }
        Status = status;
    }
}
