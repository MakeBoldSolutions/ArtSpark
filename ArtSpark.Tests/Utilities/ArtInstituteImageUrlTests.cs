using ArtSpark.Web.Utilities;

namespace ArtSpark.Tests.Utilities;

public class ArtInstituteImageUrlTests
{
    [Fact]
    public void Build_ReturnsOfficialIiifUrl()
    {
        var url = ArtInstituteImageUrl.Build("607d4ff1-578f-79ee-9980-ed133c743e64", "400,");

        Assert.Equal(
            "https://www.artic.edu/iiif/2/607d4ff1-578f-79ee-9980-ed133c743e64/full/400,/0/default.jpg",
            url);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Build_ReturnsEmptyString_WhenImageIdMissing(string? imageId)
    {
        Assert.Equal(string.Empty, ArtInstituteImageUrl.Build(imageId));
    }

    [Theory]
    [InlineData("400/../../bad")]
    [InlineData("400,abc")]
    public void Build_ReturnsEmptyString_WhenSizeInvalid(string size)
    {
        Assert.Equal(string.Empty, ArtInstituteImageUrl.Build("607d4ff1-578f-79ee-9980-ed133c743e64", size));
    }
}
