using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Net;
using System.Net.Http.Headers;
using ArtSpark.Client.Interfaces;
using ArtSpark.Web.Controllers.Api;

namespace ArtSpark.Tests.Controllers;

public class MediaApiControllerTests
{
    [Fact]
    public async Task GetIiifImage_ValidRequest_ReturnsImageFromProxy()
    {
        var client = new Mock<IArtInstituteClient>();
        var factory = new Mock<IHttpClientFactory>();
        var logger = new Mock<ILogger<MediaApiController>>();
        var handler = new StubHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new ByteArrayContent([1, 2, 3])
            {
                Headers =
                {
                    ContentType = new MediaTypeHeaderValue("image/jpeg")
                }
            }
        });

        client
            .Setup(x => x.BuildIiifUrl("607d4ff1-578f-79ee-9980-ed133c743e64", "400,", "jpg"))
            .Returns("https://www.artic.edu/iiif/2/607d4ff1-578f-79ee-9980-ed133c743e64/full/400,/0/default.jpg");
        factory
            .Setup(x => x.CreateClient("ArtInstituteImages"))
            .Returns(new HttpClient(handler));

        var controller = new MediaApiController(client.Object, factory.Object, logger.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };

        var result = await controller.GetIiifImage("607d4ff1-578f-79ee-9980-ed133c743e64", "400,", "jpg");

        var file = Assert.IsType<FileContentResult>(result);
        Assert.Equal("image/jpeg", file.ContentType);
        Assert.Equal([1, 2, 3], file.FileContents);
        Assert.Equal("https://www.artic.edu/iiif/2/607d4ff1-578f-79ee-9980-ed133c743e64/full/400,/0/default.jpg", handler.Request?.RequestUri?.ToString());
        Assert.Equal("public,max-age=2592000", controller.Response.Headers.CacheControl);
    }

    [Fact]
    public async Task GetIiifImage_InvalidSize_ReturnsBadRequest()
    {
        var client = new Mock<IArtInstituteClient>();
        var factory = new Mock<IHttpClientFactory>();
        var logger = new Mock<ILogger<MediaApiController>>();
        var controller = new MediaApiController(client.Object, factory.Object, logger.Object);

        var result = await controller.GetIiifImage("607d4ff1-578f-79ee-9980-ed133c743e64", "../400,", "jpg");

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task GetIiifImage_UpstreamForbidden_ReturnsNoContent()
    {
        var client = new Mock<IArtInstituteClient>();
        var factory = new Mock<IHttpClientFactory>();
        var logger = new Mock<ILogger<MediaApiController>>();
        var handler = new StubHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.Forbidden));

        client
            .Setup(x => x.BuildIiifUrl("607d4ff1-578f-79ee-9980-ed133c743e64", "400,", "jpg"))
            .Returns("https://www.artic.edu/iiif/2/607d4ff1-578f-79ee-9980-ed133c743e64/full/400,/0/default.jpg");
        factory
            .Setup(x => x.CreateClient("ArtInstituteImages"))
            .Returns(new HttpClient(handler));

        var controller = new MediaApiController(client.Object, factory.Object, logger.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };

        var result = await controller.GetIiifImage("607d4ff1-578f-79ee-9980-ed133c743e64", "400,", "jpg");

        Assert.IsType<NoContentResult>(result);
        Assert.Equal("no-store", controller.Response.Headers.CacheControl);
    }

    private sealed class StubHttpMessageHandler(HttpResponseMessage response) : HttpMessageHandler
    {
        public HttpRequestMessage? Request { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Request = request;
            return Task.FromResult(response);
        }
    }
}
