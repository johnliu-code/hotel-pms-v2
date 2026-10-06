using HotelPms.Domain.Financials;
using HotelPms.Domain.Properties;
using HotelPms.Domain.Rooms;

namespace HotelPms.UnitTests.Rooms;

public sealed class RoomTests
{
    private readonly Property _property = new("Retreat", "America/Vancouver", new Currency("CAD"), true);
    private RoomType CreateType() => new(_property, "UNIT", "Accommodation", 2, true);

    [Theory]
    [InlineData("101")]
    [InlineData("A-12")]
    [InlineData("CABIN-A")]
    [InlineData("CHALET-3")]
    public void PreservesStringUnitCodesAndOptionalDetails(string code)
    {
        var type = CreateType();
        var room = new Room(_property, type, code, RoomOperationalStatus.Ready, true, "Ground", "Detached unit");
        Assert.Same(_property, room.Property);
        Assert.Same(type, room.RoomType);
        Assert.Equal(code, room.Code);
        Assert.Equal("Ground", room.Floor);
        Assert.Equal("Detached unit", room.Notes);
    }

    [Theory]
    [InlineData(RoomOperationalStatus.Ready, true)]
    [InlineData(RoomOperationalStatus.Ready, false)]
    [InlineData(RoomOperationalStatus.OutOfService, true)]
    [InlineData(RoomOperationalStatus.OutOfService, false)]
    public void ActiveStateAndOperationalStatusAreIndependent(RoomOperationalStatus status, bool active)
    {
        var room = new Room(_property, CreateType(), "CABIN-A", status, active);
        Assert.Equal(status, room.OperationalStatus);
        Assert.Equal(active, room.IsActive);
        Assert.Null(room.Floor);
        Assert.Null(room.Notes);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" \t")]
    public void RejectsMissingCode(string? code)
    {
        Assert.Equal("code", Assert.ThrowsAny<ArgumentException>(() =>
            new Room(_property, CreateType(), code!, RoomOperationalStatus.Ready, true)).ParamName);
    }

    [Fact]
    public void RejectsMissingReferences()
    {
        Assert.Equal("property", Assert.Throws<ArgumentNullException>(() =>
            new Room(null!, CreateType(), "101", RoomOperationalStatus.Ready, true)).ParamName);
        Assert.Equal("roomType", Assert.Throws<ArgumentNullException>(() =>
            new Room(_property, null!, "101", RoomOperationalStatus.Ready, true)).ParamName);
    }

    [Fact]
    public void RejectsTypeFromDifferentPropertyEvenWithMatchingFields()
    {
        var other = new Property("Retreat", "America/Vancouver", new Currency("CAD"), true);
        var type = new RoomType(other, "UNIT", "Accommodation", 2, true);
        Assert.Equal("roomType", Assert.Throws<ArgumentException>(() =>
            new Room(_property, type, "101", RoomOperationalStatus.Ready, true)).ParamName);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(2)]
    public void RejectsUndefinedOperationalStatus(int status)
    {
        Assert.Equal("operationalStatus", Assert.Throws<ArgumentOutOfRangeException>(() =>
            new Room(_property, CreateType(), "101", (RoomOperationalStatus)status, true)).ParamName);
    }
}
