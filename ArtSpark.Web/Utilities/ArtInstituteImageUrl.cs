using System.Text.RegularExpressions;

namespace ArtSpark.Web.Utilities;

public static class ArtInstituteImageUrl
{
    private const string IiifBaseUrl = "https://www.artic.edu/iiif/2";
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
        if (string.IsNullOrWhiteSpace(imageId)
            || string.IsNullOrWhiteSpace(size)
            || !IiifSizePattern.IsMatch(size)
            || !AllowedImageFormats.Contains(format))
        {
            return string.Empty;
        }

        return $"{IiifBaseUrl}/{Uri.EscapeDataString(imageId)}/full/{size}/0/default.{format.ToLowerInvariant()}";
    }

    public static string BuildOfficial(string? imageId, string size = "843,", string format = "jpg") =>
        Build(imageId, size, format);
}
