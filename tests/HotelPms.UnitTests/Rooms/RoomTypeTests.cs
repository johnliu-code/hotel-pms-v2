using HotelPms.Domain.Financials;
using HotelPms.Domain.Properties;
using HotelPms.Domain.Rooms;

namespace HotelPms.UnitTests.Rooms;

public sealed class RoomTypeTests
{
    private static Property CreateProperty() => new("Retreat", "America/Vancouver", new Currency("CAD"), true);

    [Theory]
    [InlineData(true, null)]
    [InlineData(false, "Detached chalet")]
    public void PreservesPropertyAndTypeDetails(bool active, string? description)
    {
        var property = CreateProperty();
        var type = new RoomType(property, "CHALET", "Chalet", 4, active, description);
        Assert.Same(property, type.Property);
        Assert.Equal("CHALET", type.Code);
        Assert.Equal("Chalet", type.Name);
        Assert.Equal(4, type.OccupancyLimit);
        Assert.Equal(active, type.IsActive);
        Assert.Equal(description, type.Description);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" \t")]
    public void RejectsMissingCodeAndName(string? value)
    {
        Assert.Equal("code", Assert.ThrowsAny<ArgumentException>(() =>
            new RoomType(CreateProperty(), value!, "Chalet", 1, true)).ParamName);
        Assert.Equal("name", Assert.ThrowsAny<ArgumentException>(() =>
            new RoomType(CreateProperty(), "CHALET", value!, 1, true)).ParamName);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void RejectsNonPositiveOccupancy(int limit)
    {
        Assert.Equal("occupancyLimit", Assert.Throws<ArgumentOutOfRangeException>(() =>
            new RoomType(CreateProperty(), "CHALET", "Chalet", limit, true)).ParamName);
    }

    [Fact]
    public void AcceptsSingleOccupant()
    {
        Assert.Equal(1, new RoomType(CreateProperty(), "SINGLE", "Single", 1, true).OccupancyLimit);
    }

    [Fact]
    public void RejectsMissingProperty()
    {
        Assert.Equal("property", Assert.Throws<ArgumentNullException>(() =>
            new RoomType(null!, "CHALET", "Chalet", 1, true)).ParamName);
    }
}
