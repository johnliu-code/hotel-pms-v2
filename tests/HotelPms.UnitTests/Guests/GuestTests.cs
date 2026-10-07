using HotelPms.Domain.Guests;

namespace HotelPms.UnitTests.Guests;

public sealed class GuestTests
{
    [Theory]
    [InlineData("Alex")]
    [InlineData("李 明")]
    [InlineData(" Ana María ")]
    public void PreservesNameWithoutSplittingOrNormalization(string name)
    {
        Assert.Equal(name, new Guest(name).Name);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" \t")]
    public void RejectsMissingName(string? name)
    {
        Assert.Equal("name", Assert.ThrowsAny<ArgumentException>(() => new Guest(name!)).ParamName);
    }

    [Fact]
    public void MatchingNamesDoNotMergeDistinctGuests()
    {
        var first = new Guest("Alex");
        var second = new Guest("Alex");
        Assert.NotEqual(first, second);
        Assert.Equal(2, new HashSet<Guest> { first, second, first }.Count);
    }
}
