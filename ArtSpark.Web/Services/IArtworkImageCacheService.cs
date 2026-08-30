namespace ArtSpark.Web.Services;

public interface IArtworkImageCacheService
{
    string GetImageUrl(string? imageId, string size = "400,", string format = "jpg");

    Task<ArtworkImageCacheResult> CacheImageAsync(
        string? imageId,
        string size = "400,",
        string format = "jpg",
        CancellationToken cancellationToken = default);
}

public sealed record ArtworkImageCacheResult(
    bool Success,
    string ImageUrl,
    bool IsCached,
    string? ErrorMessage = null);
