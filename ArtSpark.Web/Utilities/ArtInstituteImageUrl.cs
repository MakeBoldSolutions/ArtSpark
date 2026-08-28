using System.Text.RegularExpressions;

namespace ArtSpark.Web.Utilities;

public static class ArtInstituteImageUrl
{
    private const string IiifBaseUrl = "https://www.artic.edu/iiif/2";
    private const string IiifProxyBaseUrl = "/api/media/images/iiif";
    private static readonly Regex IiifSizePattern = new(@"^\d+,?(\d+)?$", RegexOptions.Compiled);
    private static readonly HashSet<string> AllowedImageFormats = new(StringComparer.OrdinalIgnoreCase)
    {
        "jpg",
        "jpeg",
        "png",
        "webp"
    };

    public static string Build(string? imageId, string size = "843,", string format = "jpg")
    {
        if (!IsValid(imageId, size, format))
        {
            return string.Empty;
        }

        return $"{IiifProxyBaseUrl}/{Uri.EscapeDataString(imageId!)}/full/{size}/0/default.{format.ToLowerInvariant()}";
    }

    public static string BuildOfficial(string? imageId, string size = "843,", string format = "jpg")
    {
        if (string.IsNullOrWhiteSpace(imageId)
            || string.IsNullOrWhiteSpace(size)
            || !IiifSizePattern.IsMatch(size)
            || !AllowedImageFormats.Contains(format))
        {
            return string.Empty;
        }

        return $"{IiifBaseUrl}/{Uri.EscapeDataString(imageId)}/full/{size}/0/default.{format.ToLowerInvariant()}";
    }

    public static string ToOfficialUrl(string? imageUrl)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
        {
            return string.Empty;
        }

        if (imageUrl.StartsWith(IiifProxyBaseUrl + "/", StringComparison.OrdinalIgnoreCase))
        {
            return IiifBaseUrl + imageUrl[IiifProxyBaseUrl.Length..];
        }

        return imageUrl;
    }

    private static bool IsValid(string? imageId, string size, string format)
    {
        return !string.IsNullOrWhiteSpace(imageId)
            && !string.IsNullOrWhiteSpace(size)
            && IiifSizePattern.IsMatch(size)
            && AllowedImageFormats.Contains(format);
    }
}
