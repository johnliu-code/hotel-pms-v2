using HotelPms.Domain.Customers;
using HotelPms.Domain.Financials;
using HotelPms.Domain.Guests;
using HotelPms.Domain.Properties;
using HotelPms.Domain.Reservations;
using HotelPms.Domain.Rooms;

namespace HotelPms.UnitTests.Reservations;

public sealed class ReservationTests
{
    private readonly Property _property = new("Retreat", "UTC", new Currency("CAD"), true);
    private readonly Customer _customer = new("Booking contact");
    private static readonly DateOnly Arrival = new(2026, 10, 6);
    private RoomType Type(int capacity = 4) => new(_property, "CABIN", "Cabin", capacity, true);
    private Reservation Create(IEnumerable<ReservationGuest>? guests = null) =>
        new(_property, Type(), _customer, Arrival, Arrival.AddDays(1), 1, 0,
            EntryMethod.Manual, guests);

    [Fact]
    public void CreatesDraftWithExclusiveDateOnlyStayAndRelationships()
    {
        var type = Type();
        var reservation = new Reservation(_property, type, _customer, Arrival, Arrival.AddDays(1),
            2, 2, EntryMethod.Import, bookingChannelCode: " custom-channel ", notes: " plain text ");
        Assert.Same(_property, reservation.Property);
        Assert.Same(type, reservation.RoomType);
        Assert.Same(_customer, reservation.Customer);
        Assert.Equal(Arrival, reservation.CheckInDate);
        Assert.Equal(Arrival.AddDays(1), reservation.CheckOutDate);
        Assert.Equal(2, reservation.Adults);
        Assert.Equal(2, reservation.Children);
        Assert.Equal(EntryMethod.Import, reservation.EntryMethod);
        Assert.Equal(" custom-channel ", reservation.BookingChannelCode);
        Assert.Equal(" plain text ", reservation.Notes);
        Assert.Equal(ReservationStatus.Draft, reservation.Status);
        Assert.Null(reservation.ConfirmationNumber);
        Assert.Empty(reservation.Guests);
    }

    [Fact]
    public void RejectsNullRelationships()
    {
        Assert.Throws<ArgumentNullException>(() => new Reservation(null!, Type(), _customer, Arrival, Arrival.AddDays(1), 1, 0, EntryMethod.Manual));
        Assert.Throws<ArgumentNullException>(() => new Reservation(_property, null!, _customer, Arrival, Arrival.AddDays(1), 1, 0, EntryMethod.Manual));
        Assert.Throws<ArgumentNullException>(() => new Reservation(_property, Type(), null!, Arrival, Arrival.AddDays(1), 1, 0, EntryMethod.Manual));
    }

    [Fact]
    public void RejectsRoomTypeFromAnotherPropertyWithMatchingFields()
    {
        var other = new Property("Retreat", "UTC", new Currency("CAD"), true);
        var type = new RoomType(other, "CABIN", "Cabin", 4, true);
        Assert.Throws<ArgumentException>(() => new Reservation(_property, type, _customer, Arrival, Arrival.AddDays(1), 1, 0, EntryMethod.Manual));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void RejectsNonPositiveStayLength(int days)
    {
        Assert.Throws<ArgumentException>(() => new Reservation(_property, Type(), _customer, Arrival, Arrival.AddDays(days), 1, 0, EntryMethod.Manual));
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(-1, 0)]
    [InlineData(1, -1)]
    [InlineData(5, 0)]
    [InlineData(2, 3)]
    [InlineData(int.MaxValue, int.MaxValue)]
    public void RejectsInvalidOrExcessiveOccupancy(int adults, int children)
    {
        Assert.ThrowsAny<ArgumentException>(() => new Reservation(_property, Type(), _customer, Arrival, Arrival.AddDays(1), adults, children, EntryMethod.Manual));
    }

    [Theory]
    [InlineData(1, 0)]
    [InlineData(4, 0)]
    [InlineData(1, 3)]
    public void AcceptsOccupancyAtOrBelowLimit(int adults, int children)
    {
        var reservation = new Reservation(_property, Type(), _customer, Arrival, Arrival.AddDays(1), adults, children, EntryMethod.Manual);
        Assert.Equal(adults, reservation.Adults);
        Assert.Equal(children, reservation.Children);
    }

    [Theory]
    [InlineData(EntryMethod.Manual)]
    [InlineData(EntryMethod.Import)]
    [InlineData(EntryMethod.Integration)]
    public void EntryMethodDoesNotRequireBookingChannel(EntryMethod method)
    {
        var reservation = new Reservation(_property, Type(), _customer, Arrival, Arrival.AddDays(1), 1, 0, method);
        Assert.Equal(method, reservation.EntryMethod);
        Assert.Null(reservation.BookingChannelCode);
        Assert.Null(reservation.Notes);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(3)]
    public void RejectsUndefinedEntryMethod(int method)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Reservation(_property, Type(), _customer, Arrival, Arrival.AddDays(1), 1, 0, (EntryMethod)method));
    }

    [Fact]
    public void CopiesGuestCollectionAndRetainsGuestReferences()
    {
        var guest = new Guest("Staying person");
        var primary = new ReservationGuest(guest, true);
        var source = new List<ReservationGuest> { primary };
        var reservation = Create(source);
        source.Clear();
        reservation.AddGuest(new ReservationGuest(new Guest("Second person"), false));
        Assert.Equal(2, reservation.Guests.Count);
        Assert.Same(primary, reservation.Guests[0]);
        Assert.Same(guest, reservation.Guests[0].Guest);
        Assert.NotEqual<object>(reservation.Customer, guest);
        Assert.Throws<NotSupportedException>(() => ((IList<ReservationGuest>)reservation.Guests).Clear());
    }

    [Fact]
    public void RejectsSecondPrimaryWithoutChangingCollection()
    {
        var first = new ReservationGuest(new Guest("First"), true);
        var reservation = Create([first]);
        Assert.Throws<ArgumentException>(() => reservation.AddGuest(new ReservationGuest(new Guest("Second"), true)));
        Assert.Single(reservation.Guests);
        Assert.Same(first, reservation.Guests[0]);
    }

    [Fact]
    public void RejectsInvalidInitialGuestAssociations()
    {
        Assert.Throws<ArgumentNullException>(() => Create([null!]));
        Assert.Throws<ArgumentException>(() => Create([
            new ReservationGuest(new Guest("First"), true),
            new ReservationGuest(new Guest("Second"), true)]));
        var reservation = Create();
        Assert.Throws<ArgumentNullException>(() => reservation.AddGuest(null!));
        Assert.Empty(reservation.Guests);
    }

    [Fact]
    public void AllowsMultipleGuestsWithoutRequiringPrimaryDesignation()
    {
        var reservation = Create([
            new ReservationGuest(new Guest("First"), false),
            new ReservationGuest(new Guest("Second"), false)]);
        Assert.Equal(2, reservation.Guests.Count);
        Assert.All(reservation.Guests, guest => Assert.False(guest.IsPrimary));
    }

    public static IEnumerable<object[]> Transitions()
    {
        foreach (var from in Enum.GetValues<ReservationStatus>())
        foreach (var to in Enum.GetValues<ReservationStatus>())
        {
            bool allowed = (from, to) is
                (ReservationStatus.Draft, ReservationStatus.Confirmed) or
                (ReservationStatus.Draft, ReservationStatus.Cancelled) or
                (ReservationStatus.Confirmed, ReservationStatus.CheckedIn) or
                (ReservationStatus.Confirmed, ReservationStatus.Cancelled) or
                (ReservationStatus.CheckedIn, ReservationStatus.CheckedOut);
            yield return [from, to, allowed];
        }
    }

    private Reservation At(ReservationStatus status)
    {
        var reservation = Create();
        if (status == ReservationStatus.Cancelled)
            reservation.TransitionTo(status);
        else if (status != ReservationStatus.Draft)
        {
            reservation.TransitionTo(ReservationStatus.Confirmed, "CONF-1");
            if (status is ReservationStatus.CheckedIn or ReservationStatus.CheckedOut)
                reservation.TransitionTo(ReservationStatus.CheckedIn);
            if (status == ReservationStatus.CheckedOut)
                reservation.TransitionTo(ReservationStatus.CheckedOut);
        }
        return reservation;
    }

    [Theory]
    [MemberData(nameof(Transitions))]
    public void EnforcesCompleteLifecycleTransitionMatrix(ReservationStatus from, ReservationStatus to, bool allowed)
    {
        var reservation = At(from);
        var previousNumber = reservation.ConfirmationNumber;
        if (allowed)
        {
            reservation.TransitionTo(to, to == ReservationStatus.Confirmed ? "CONF-1" : null);
            Assert.Equal(to, reservation.Status);
            Assert.Equal(to == ReservationStatus.Confirmed ? "CONF-1" : previousNumber, reservation.ConfirmationNumber);
        }
        else
        {
            Assert.Throws<InvalidOperationException>(() => reservation.TransitionTo(to));
            Assert.Equal(from, reservation.Status);
            Assert.Equal(previousNumber, reservation.ConfirmationNumber);
        }
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void ConfirmationRequiresNonBlankNumberAndFailureIsAtomic(string? number)
    {
        var reservation = Create();
        Assert.ThrowsAny<ArgumentException>(() => reservation.TransitionTo(ReservationStatus.Confirmed, number));
        Assert.Equal(ReservationStatus.Draft, reservation.Status);
        Assert.Null(reservation.ConfirmationNumber);
        reservation.TransitionTo(ReservationStatus.Confirmed, "VALID");
        Assert.Equal("VALID", reservation.ConfirmationNumber);
    }

    [Fact]
    public void NewReservationHasNoConfirmationNumberAndRequiresOneToConfirm()
    {
        var reservation = Create();
        Assert.Equal(ReservationStatus.Draft, reservation.Status);
        Assert.Null(reservation.ConfirmationNumber);
        Assert.Throws<ArgumentNullException>(() => reservation.TransitionTo(ReservationStatus.Confirmed));
        Assert.Equal(ReservationStatus.Draft, reservation.Status);
        Assert.Null(reservation.ConfirmationNumber);
    }

    [Fact]
    public void ConfirmationAssignsNumberAndStayTransitionsRetainIt()
    {
        var reservation = Create();
        reservation.TransitionTo(ReservationStatus.Confirmed, " number as supplied ");
        Assert.Equal(ReservationStatus.Confirmed, reservation.Status);
        Assert.Equal(" number as supplied ", reservation.ConfirmationNumber);
        reservation.TransitionTo(ReservationStatus.CheckedIn);
        Assert.Equal(" number as supplied ", reservation.ConfirmationNumber);
        reservation.TransitionTo(ReservationStatus.CheckedOut);
        Assert.Equal(" number as supplied ", reservation.ConfirmationNumber);
    }

    [Fact]
    public void CancellingUnconfirmedDraftKeepsNumberNull()
    {
        var reservation = Create();
        reservation.TransitionTo(ReservationStatus.Cancelled);
        Assert.Equal(ReservationStatus.Cancelled, reservation.Status);
        Assert.Null(reservation.ConfirmationNumber);
    }

    [Fact]
    public void RejectsConfirmationInputOutsideConfirmationWithoutMutation()
    {
        var reservation = At(ReservationStatus.Confirmed);
        Assert.Throws<ArgumentException>(() => reservation.TransitionTo(ReservationStatus.CheckedIn, "REPLACEMENT"));
        Assert.Equal(ReservationStatus.Confirmed, reservation.Status);
        Assert.Equal("CONF-1", reservation.ConfirmationNumber);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(5)]
    public void RejectsUndefinedLifecycleState(int status)
    {
        var reservation = Create();
        Assert.Throws<InvalidOperationException>(() => reservation.TransitionTo((ReservationStatus)status));
        Assert.Equal(ReservationStatus.Draft, reservation.Status);
    }

    [Fact]
    public void CancellationRetainsBookingAndGuests()
    {
        var association = new ReservationGuest(new Guest("Person"), true);
        var reservation = Create([association]);
        reservation.TransitionTo(ReservationStatus.Confirmed, "CONF-1");
        reservation.TransitionTo(ReservationStatus.Cancelled);
        Assert.Same(_property, reservation.Property);
        Assert.Same(_customer, reservation.Customer);
        Assert.Same(association, Assert.Single(reservation.Guests));
        Assert.Equal(Arrival, reservation.CheckInDate);
        Assert.Equal("CONF-1", reservation.ConfirmationNumber);
    }
}
