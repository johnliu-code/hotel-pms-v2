using HotelPms.Domain.Guests;
using HotelPms.Domain.Reservations;

namespace HotelPms.UnitTests.Reservations;

public sealed class ReservationGuestTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void RetainsGuestReferenceAndPrimaryDesignation(bool isPrimary)
    {
        var guest = new Guest("Alex");
        var association = new ReservationGuest(guest, isPrimary);
        Assert.Same(guest, association.Guest);
        Assert.Equal(isPrimary, association.IsPrimary);
    }

    [Fact]
    public void RejectsMissingGuest()
    {
        Assert.Equal("guest", Assert.Throws<ArgumentNullException>(() =>
            new ReservationGuest(null!, true)).ParamName);
    }

    [Fact]
    public void PrimaryDesignationIsLocalToEachAssociation()
    {
        var first = new ReservationGuest(new Guest("Alex"), true);
        var second = new ReservationGuest(new Guest("Morgan"), true);
        Assert.True(first.IsPrimary);
        Assert.True(second.IsPrimary);
    }

    [Fact]
    public void DoesNotApplyGlobalPrimaryDesignationToGuest()
    {
        var guest = new Guest("Alex");
        var primary = new ReservationGuest(guest, true);
        var additional = new ReservationGuest(guest, false);
        Assert.Same(primary.Guest, additional.Guest);
        Assert.True(primary.IsPrimary);
        Assert.False(additional.IsPrimary);
    }
}
