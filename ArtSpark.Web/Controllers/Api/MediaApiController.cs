using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using ArtSpark.Client.Interfaces;
using ArtSpark.Client.Models.Collections;
using ArtSpark.Client.Models.Common;
using ArtSpark.Client.Models.Mobile;
using CollectionImage = ArtSpark.Client.Models.Collections.Image;

namespace ArtSpark.Demo.Controllers.Api;

/// <summary>
/// API controller for media-related operations (images, videos, sounds, mobile sounds)
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Route("api/media")]
public class MediaApiController : ControllerBase
{
    private readonly IArtInstituteClient _artInstituteClient;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<MediaApiController> _logger;
    private static readonly Regex ImageIdPattern = new("^[a-zA-Z0-9-]+$", RegexOptions.Compiled);
    private static readonly Regex IiifSizePattern = new(@"^\d+,?(\d+)?$", RegexOptions.Compiled);
    private static readonly HashSet<string> AllowedImageFormats = new(StringComparer.OrdinalIgnoreCase)
    {
        "jpg",
        "jpeg",
        "png",
        "webp"
    };

    public MediaApiController(
        IArtInstituteClient artInstituteClient,
        IHttpClientFactory httpClientFactory,
        ILogger<MediaApiController> logger)
    {
        _artInstituteClient = artInstituteClient ?? throw new ArgumentNullException(nameof(artInstituteClient));
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Build IIIF URL for an image
    /// </summary>
    /// <param name="imageId">Image ID</param>
    /// <param name="size">Image size (default: "843,")</param>
    /// <param name="format">Image format (default: "jpg")</param>
    /// <returns>IIIF URL</returns>
    [HttpGet("images/iiif-url/{imageId}")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<string> BuildIiifUrl(
        string imageId,
        [FromQuery] string size = "843,",
        [FromQuery] string format = "jpg")
    {
        try
        {
            if (string.IsNullOrWhiteSpace(imageId))
            {
                return BadRequest("Image ID is required");
            }

            var url = _artInstituteClient.BuildIiifUrl(imageId, size, format);
            return Ok(url);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error building IIIF URL for image {ImageId}", imageId);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while building the IIIF URL");
        }
    }

    /// <summary>
    /// Proxy an Art Institute IIIF image through the app origin.
    /// </summary>
    [HttpGet("images/iiif/{imageId}/full/{size}/0/default.{format}")]
    [HttpHead("images/iiif/{imageId}/full/{size}/0/default.{format}")]
    [ResponseCache(Duration = 2592000, Location = ResponseCacheLocation.Any)]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status502BadGateway)]
    public async Task<IActionResult> GetIiifImage(
        string imageId,
        string size,
        string format = "jpg",
        CancellationToken cancellationToken = default)
    {
        if (!IsValidIiifRequest(imageId, size, format))
        {
            return BadRequest("Invalid image request");
        }

        var imageUrl = _artInstituteClient.BuildIiifUrl(imageId, size, format);
        var httpClient = _httpClientFactory.CreateClient("ArtInstituteImages");

        try
        {
            using var response = await httpClient.GetAsync(imageUrl, cancellationToken).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("IIIF image request failed for {ImageId} with status {StatusCode}", imageId, response.StatusCode);
                return StatusCode((int)response.StatusCode);
            }

            var contentType = response.Content.Headers.ContentType?.ToString() ?? GetContentType(format);
            if (!contentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning("IIIF image request for {ImageId} returned unexpected content type {ContentType}", imageId, contentType);
                return StatusCode(StatusCodes.Status502BadGateway);
            }

            var imageBytes = await response.Content.ReadAsByteArrayAsync(cancellationToken).ConfigureAwait(false);
            Response.Headers.CacheControl = "public,max-age=2592000";
            return File(imageBytes, contentType);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            return new EmptyResult();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error proxying IIIF image {ImageId}", imageId);
            return StatusCode(StatusCodes.Status502BadGateway);
        }
    }

    /// <summary>
    /// Get a specific image by ID
    /// </summary>
    /// <param name="id">Image ID</param>
    /// <param name="fields">Optional fields to include</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Image details</returns>
    [HttpGet("images/{id}")]
    [ProducesResponseType(typeof(CollectionImage), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CollectionImage>> GetImage(
        string id,
        [FromQuery] string[]? fields = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var image = await _artInstituteClient.GetImageAsync(id, fields, cancellationToken);

            if (image == null)
            {
                _logger.LogWarning("Image with ID {ImageId} not found", id);
                return NotFound($"Image with ID {id} not found");
            }

            return Ok(image);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving image with ID {ImageId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving the image");
        }
    }

    /// <summary>
    /// Get all images with optional query parameters
    /// </summary>
    /// <param name="limit">Number of results to return</param>
    /// <param name="page">Page number</param>
    /// <param name="fields">Optional fields to include</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of images</returns>
    [HttpGet("images")]
    [ProducesResponseType(typeof(ApiResponse<CollectionImage>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<CollectionImage>>> GetImages(
        [FromQuery] int? limit = null,
        [FromQuery] int? page = null,
        [FromQuery] string[]? fields = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new ApiQuery
            {
                Limit = limit,
                Page = page,
                Fields = fields != null ? string.Join(",", fields) : null
            };

            var result = await _artInstituteClient.GetImagesAsync(query, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving images");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving images");
        }
    }

    /// <summary>
    /// Search images using advanced query
    /// </summary>
    /// <param name="query">Search query</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Search results for images</returns>
    [HttpPost("images/search")]
    [ProducesResponseType(typeof(SearchResponse<CollectionImage>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SearchResponse<CollectionImage>>> SearchImages(
        [FromBody] SearchQuery query,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (query == null)
            {
                return BadRequest("Search query is required");
            }

            var result = await _artInstituteClient.SearchImagesAsync(query, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching images with advanced query");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while searching images");
        }
    }

    /// <summary>
    /// Get a specific video by ID
    /// </summary>
    /// <param name="id">Video ID</param>
    /// <param name="fields">Optional fields to include</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Video details</returns>
    [HttpGet("videos/{id}")]
    [ProducesResponseType(typeof(Video), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Video>> GetVideo(
        string id,
        [FromQuery] string[]? fields = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var video = await _artInstituteClient.GetVideoAsync(id, fields, cancellationToken);

            if (video == null)
            {
                _logger.LogWarning("Video with ID {VideoId} not found", id);
                return NotFound($"Video with ID {id} not found");
            }

            return Ok(video);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving video with ID {VideoId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving the video");
        }
    }

    /// <summary>
    /// Get all videos with optional query parameters
    /// </summary>
    /// <param name="limit">Number of results to return</param>
    /// <param name="page">Page number</param>
    /// <param name="fields">Optional fields to include</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of videos</returns>
    [HttpGet("videos")]
    [ProducesResponseType(typeof(ApiResponse<Video>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<Video>>> GetVideos(
        [FromQuery] int? limit = null,
        [FromQuery] int? page = null,
        [FromQuery] string[]? fields = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new ApiQuery
            {
                Limit = limit,
                Page = page,
                Fields = fields != null ? string.Join(",", fields) : null
            };

            var result = await _artInstituteClient.GetVideosAsync(query, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving videos");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving videos");
        }
    }

    /// <summary>
    /// Search videos using advanced query
    /// </summary>
    /// <param name="query">Search query</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Search results for videos</returns>
    [HttpPost("videos/search")]
    [ProducesResponseType(typeof(SearchResponse<Video>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SearchResponse<Video>>> SearchVideos(
        [FromBody] SearchQuery query,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (query == null)
            {
                return BadRequest("Search query is required");
            }

            var result = await _artInstituteClient.SearchVideosAsync(query, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching videos with advanced query");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while searching videos");
        }
    }

    /// <summary>
    /// Get a specific sound by ID
    /// </summary>
    /// <param name="id">Sound ID</param>
    /// <param name="fields">Optional fields to include</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Sound details</returns>
    [HttpGet("sounds/{id}")]
    [ProducesResponseType(typeof(Sound), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Sound>> GetSound(
        string id,
        [FromQuery] string[]? fields = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var sound = await _artInstituteClient.GetSoundAsync(id, fields, cancellationToken);

            if (sound == null)
            {
                _logger.LogWarning("Sound with ID {SoundId} not found", id);
                return NotFound($"Sound with ID {id} not found");
            }

            return Ok(sound);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving sound with ID {SoundId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving the sound");
        }
    }

    /// <summary>
    /// Get all sounds with optional query parameters
    /// </summary>
    /// <param name="limit">Number of results to return</param>
    /// <param name="page">Page number</param>
    /// <param name="fields">Optional fields to include</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of sounds</returns>
    [HttpGet("sounds")]
    [ProducesResponseType(typeof(ApiResponse<Sound>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<Sound>>> GetSounds(
        [FromQuery] int? limit = null,
        [FromQuery] int? page = null,
        [FromQuery] string[]? fields = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new ApiQuery
            {
                Limit = limit,
                Page = page,
                Fields = fields != null ? string.Join(",", fields) : null
            };

            var result = await _artInstituteClient.GetSoundsAsync(query, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving sounds");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving sounds");
        }
    }

    /// <summary>
    /// Search sounds using advanced query
    /// </summary>
    /// <param name="query">Search query</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Search results for sounds</returns>
    [HttpPost("sounds/search")]
    [ProducesResponseType(typeof(SearchResponse<Sound>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SearchResponse<Sound>>> SearchSounds(
        [FromBody] SearchQuery query,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (query == null)
            {
                return BadRequest("Search query is required");
            }

            var result = await _artInstituteClient.SearchSoundsAsync(query, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching sounds with advanced query");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while searching sounds");
        }
    }

    /// <summary>
    /// Get a specific mobile sound by ID
    /// </summary>
    /// <param name="id">Mobile sound ID</param>
    /// <param name="fields">Optional fields to include</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Mobile sound details</returns>
    [HttpGet("mobile-sounds/{id:int}")]
    [ProducesResponseType(typeof(MobileSound), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<MobileSound>> GetMobileSound(
        int id,
        [FromQuery] string[]? fields = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var mobileSound = await _artInstituteClient.GetMobileSoundAsync(id, fields, cancellationToken);

            if (mobileSound == null)
            {
                _logger.LogWarning("Mobile sound with ID {MobileSoundId} not found", id);
                return NotFound($"Mobile sound with ID {id} not found");
            }

            return Ok(mobileSound);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving mobile sound with ID {MobileSoundId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving the mobile sound");
        }
    }

    /// <summary>
    /// Get all mobile sounds with optional query parameters
    /// </summary>
    /// <param name="limit">Number of results to return</param>
    /// <param name="page">Page number</param>
    /// <param name="fields">Optional fields to include</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of mobile sounds</returns>
    [HttpGet("mobile-sounds")]
    [ProducesResponseType(typeof(ApiResponse<MobileSound>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<MobileSound>>> GetMobileSounds(
        [FromQuery] int? limit = null,
        [FromQuery] int? page = null,
        [FromQuery] string[]? fields = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new ApiQuery
            {
                Limit = limit,
                Page = page,
                Fields = fields != null ? string.Join(",", fields) : null
            };

            var result = await _artInstituteClient.GetMobileSoundsAsync(query, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving mobile sounds");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving mobile sounds");
        }
    }

    /// <summary>
    /// Search mobile sounds using advanced query
    /// </summary>
    /// <param name="query">Search query</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Search results for mobile sounds</returns>
    [HttpPost("mobile-sounds/search")]
    [ProducesResponseType(typeof(SearchResponse<MobileSound>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SearchResponse<MobileSound>>> SearchMobileSounds(
        [FromBody] SearchQuery query,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (query == null)
            {
                return BadRequest("Search query is required");
            }

            var result = await _artInstituteClient.SearchMobileSoundsAsync(query, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching mobile sounds with advanced query");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while searching mobile sounds");
        }
    }

    /// <summary>
    /// Get a specific text by ID
    /// </summary>
    /// <param name="id">Text ID</param>
    /// <param name="fields">Optional fields to include</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Text details</returns>
    [HttpGet("texts/{id}")]
    [ProducesResponseType(typeof(Text), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Text>> GetText(
        string id,
        [FromQuery] string[]? fields = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var text = await _artInstituteClient.GetTextAsync(id, fields, cancellationToken);

            if (text == null)
            {
                _logger.LogWarning("Text with ID {TextId} not found", id);
                return NotFound($"Text with ID {id} not found");
            }

            return Ok(text);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving text with ID {TextId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving the text");
        }
    }

    /// <summary>
    /// Get all texts with optional query parameters
    /// </summary>
    /// <param name="limit">Number of results to return</param>
    /// <param name="page">Page number</param>
    /// <param name="fields">Optional fields to include</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of texts</returns>
    [HttpGet("texts")]
    [ProducesResponseType(typeof(ApiResponse<Text>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<Text>>> GetTexts(
        [FromQuery] int? limit = null,
        [FromQuery] int? page = null,
        [FromQuery] string[]? fields = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new ApiQuery
            {
                Limit = limit,
                Page = page,
                Fields = fields != null ? string.Join(",", fields) : null
            };

            var result = await _artInstituteClient.GetTextsAsync(query, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving texts");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving texts");
        }
    }

    /// <summary>
    /// Search texts using advanced query
    /// </summary>
    /// <param name="query">Search query</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Search results for texts</returns>
    [HttpPost("texts/search")]
    [ProducesResponseType(typeof(SearchResponse<Text>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SearchResponse<Text>>> SearchTexts(
        [FromBody] SearchQuery query,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (query == null)
            {
                return BadRequest("Search query is required");
            }

            var result = await _artInstituteClient.SearchTextsAsync(query, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching texts with advanced query");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while searching texts");
        }
    }

    private static bool IsValidIiifRequest(string imageId, string size, string format)
    {
        return ImageIdPattern.IsMatch(imageId)
            && IiifSizePattern.IsMatch(size)
            && AllowedImageFormats.Contains(format);
    }

    private static string GetContentType(string format)
    {
        return format.Equals("jpg", StringComparison.OrdinalIgnoreCase) || format.Equals("jpeg", StringComparison.OrdinalIgnoreCase)
            ? "image/jpeg"
            : $"image/{format.ToLowerInvariant()}";
    }
}
