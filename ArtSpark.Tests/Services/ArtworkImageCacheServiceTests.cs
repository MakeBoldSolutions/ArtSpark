using ArtSpark.Web.Services;
using Microsoft.AspNetCore.Hosting;
using Moq;

namespace ArtSpark.Tests.Services;

public class ArtworkImageCacheServiceTests : IDisposable
{
    private readonly string _tempPath = Path.Combine(Path.GetTempPath(), "artspark-image-cache-tests", Guid.NewGuid().ToString("N"));

    [Fact]
    public void GetImageUrl_CachedFileExists_ReturnsLocalUrl()
    {
        var imageId = "47c5bcb8-62ef-e5d7-55e7-f5121f409a30";
        var cacheDirectory = Path.Combine(_tempPath, "img", "artworks", "400w");
        Directory.CreateDirectory(cacheDirectory);
        File.WriteAllText(Path.Combine(cacheDirectory, $"{imageId}.jpg"), "test");
        var service = CreateService();

        var url = service.GetImageUrl(imageId, "400,");

        Assert.Equal($"/img/artworks/400w/{imageId}.jpg", url);
    }

    [Fact]
    public void GetImageUrl_CachedFileMissing_ReturnsOfficialIiifUrl()
    {
        var imageId = "47c5bcb8-62ef-e5d7-55e7-f5121f409a30";
        var service = CreateService();

        var url = service.GetImageUrl(imageId, "400,");

        Assert.Equal($"https://www.artic.edu/iiif/2/{imageId}/full/400,/0/default.jpg", url);
    }

    [Theory]
    [InlineData("")]
    [InlineData("../bad")]
    public void GetImageUrl_InvalidImageId_ReturnsEmpty(string imageId)
    {
        var service = CreateService();

        var url = service.GetImageUrl(imageId, "400,");

        Assert.Equal(string.Empty, url);
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempPath))
        {
            Directory.Delete(_tempPath, recursive: true);
        }
    }

    private ArtworkImageCacheService CreateService()
    {
        var environment = new Mock<IWebHostEnvironment>();
        environment.SetupGet(x => x.WebRootPath).Returns(_tempPath);
        environment.SetupGet(x => x.ContentRootPath).Returns(_tempPath);

        var httpClientFactory = new Mock<IHttpClientFactory>();
        var logger = new Mock<ILogger<ArtworkImageCacheService>>();

        return new ArtworkImageCacheService(environment.Object, httpClientFactory.Object, logger.Object);
    }
}
