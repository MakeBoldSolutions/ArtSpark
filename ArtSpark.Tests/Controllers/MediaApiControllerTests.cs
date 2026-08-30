using Microsoft.AspNetCore.Mvc;
using Moq;
using ArtSpark.Client.Interfaces;
using ArtSpark.Web.Controllers.Api;
using ArtSpark.Web.Services;

namespace ArtSpark.Tests.Controllers;

public class MediaApiControllerTests
{
    [Fact]
    public void GetIiifImage_ValidRequest_RedirectsToOfficialIiifUrl()
    {
        var client = new Mock<IArtInstituteClient>();
        var imageCache = new Mock<IArtworkImageCacheService>();
        var logger = new Mock<ILogger<MediaApiController>>();

        client
            .Setup(x => x.BuildIiifUrl("607d4ff1-578f-79ee-9980-ed133c743e64", "400,", "jpg"))
            .Returns("https://www.artic.edu/iiif/2/607d4ff1-578f-79ee-9980-ed133c743e64/full/400,/0/default.jpg");

        var controller = new MediaApiController(client.Object, imageCache.Object, logger.Object);

        var result = controller.GetIiifImage("607d4ff1-578f-79ee-9980-ed133c743e64", "400,", "jpg");

        var redirect = Assert.IsType<RedirectResult>(result);
        Assert.Equal("https://www.artic.edu/iiif/2/607d4ff1-578f-79ee-9980-ed133c743e64/full/400,/0/default.jpg", redirect.Url);
    }

    [Fact]
    public void GetIiifImage_InvalidSize_ReturnsBadRequest()
    {
        var client = new Mock<IArtInstituteClient>();
        var imageCache = new Mock<IArtworkImageCacheService>();
        var logger = new Mock<ILogger<MediaApiController>>();
        var controller = new MediaApiController(client.Object, imageCache.Object, logger.Object);

        var result = controller.GetIiifImage("607d4ff1-578f-79ee-9980-ed133c743e64", "../400,", "jpg");

        Assert.IsType<BadRequestObjectResult>(result);
    }
}
