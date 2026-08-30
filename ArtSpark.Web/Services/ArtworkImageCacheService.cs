using System.Text.RegularExpressions;
using ArtSpark.Web.Utilities;

namespace ArtSpark.Web.Services;

public class ArtworkImageCacheService : IArtworkImageCacheService
{
    public const string RequestPath = "/img/artworks";

    private static readonly Regex ImageIdPattern = new("^[a-zA-Z0-9-]+$", RegexOptions.Compiled);
    private static readonly Regex IiifSizePattern = new(@"^\d+,?(\d+)?$", RegexOptions.Compiled);
    private static readonly HashSet<string> AllowedImageFormats = new(StringComparer.OrdinalIgnoreCase)
    {
        "jpg",
        "jpeg",
        "png",
        "webp"
    };

    private readonly IWebHostEnvironment _environment;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ArtworkImageCacheService> _logger;

    public ArtworkImageCacheService(
        IWebHostEnvironment environment,
        IHttpClientFactory httpClientFactory,
        ILogger<ArtworkImageCacheService> logger)
    {
        _environment = environment;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public string GetImageUrl(string? imageId, string size = "400,", string format = "jpg")
    {
        if (!IsValidRequest(imageId, size, format))
        {
            return string.Empty;
        }

        var localPath = GetCacheFilePath(imageId!, size, format);
        if (File.Exists(localPath))
        {
            return GetCacheUrl(imageId!, size, format);
        }

        return ArtInstituteImageUrl.Build(imageId, size, format);
    }

    public async Task<ArtworkImageCacheResult> CacheImageAsync(
        string? imageId,
        string size = "400,",
        string format = "jpg",
        CancellationToken cancellationToken = default)
    {
        if (!IsValidRequest(imageId, size, format))
        {
            return new ArtworkImageCacheResult(false, string.Empty, false, "Invalid image request");
        }

        var localPath = GetCacheFilePath(imageId!, size, format);
        var localUrl = GetCacheUrl(imageId!, size, format);

        if (File.Exists(localPath))
        {
            return new ArtworkImageCacheResult(true, localUrl, true);
        }

        var sourceUrl = ArtInstituteImageUrl.BuildOfficial(imageId, size, format);
        if (string.IsNullOrEmpty(sourceUrl))
        {
            return new ArtworkImageCacheResult(false, string.Empty, false, "Could not build source image URL");
        }

        try
        {
            var client = _httpClientFactory.CreateClient("ArtInstituteImages");
            using var response = await client.GetAsync(sourceUrl, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var message = $"Source image request failed with {(int)response.StatusCode} {response.ReasonPhrase}";
                _logger.LogWarning("Could not cache artwork image {ImageId}. {Message}", imageId, message);
                return new ArtworkImageCacheResult(false, sourceUrl, false, message);
            }

            var contentType = response.Content.Headers.ContentType?.MediaType;
            if (contentType == null || !contentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
            {
                var message = $"Source returned {contentType ?? "unknown"} instead of an image";
                _logger.LogWarning("Could not cache artwork image {ImageId}. {Message}", imageId, message);
                return new ArtworkImageCacheResult(false, sourceUrl, false, message);
            }

            Directory.CreateDirectory(Path.GetDirectoryName(localPath)!);
            var tempPath = $"{localPath}.{Guid.NewGuid():N}.tmp";

            try
            {
                await using (var fileStream = File.Create(tempPath))
                {
                    await response.Content.CopyToAsync(fileStream, cancellationToken);
                }

                File.Move(tempPath, localPath, overwrite: true);
            }
            finally
            {
                if (File.Exists(tempPath))
                {
                    File.Delete(tempPath);
                }
            }

            return new ArtworkImageCacheResult(true, localUrl, true);
        }
        catch (Exception ex) when (ex is HttpRequestException or IOException or UnauthorizedAccessException
            || ex is TaskCanceledException && !cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning(ex, "Could not cache artwork image {ImageId}", imageId);
            return new ArtworkImageCacheResult(false, sourceUrl, false, ex.Message);
        }
    }

    private static bool IsValidRequest(string? imageId, string size, string format)
    {
        return !string.IsNullOrWhiteSpace(imageId)
            && ImageIdPattern.IsMatch(imageId)
            && !string.IsNullOrWhiteSpace(size)
            && IiifSizePattern.IsMatch(size)
            && AllowedImageFormats.Contains(format);
    }

    private string GetCacheFilePath(string imageId, string size, string format)
    {
        var webRootPath = _environment.WebRootPath
            ?? Path.Combine(_environment.ContentRootPath, "wwwroot");

        return Path.Combine(
            webRootPath,
            "img",
            "artworks",
            GetSizeToken(size),
            $"{imageId}.{NormalizeFormat(format)}");
    }

    private static string GetCacheUrl(string imageId, string size, string format)
    {
        return $"{RequestPath}/{GetSizeToken(size)}/{imageId}.{NormalizeFormat(format)}";
    }

    private static string GetSizeToken(string size)
    {
        var normalized = size.Trim();
        return normalized.EndsWith(",", StringComparison.Ordinal)
            ? $"{normalized.TrimEnd(',')}w"
            : normalized.Replace(",", "x", StringComparison.Ordinal);
    }

    private static string NormalizeFormat(string format)
    {
        return format.Equals("jpeg", StringComparison.OrdinalIgnoreCase)
            ? "jpg"
            : format.ToLowerInvariant();
    }
}
