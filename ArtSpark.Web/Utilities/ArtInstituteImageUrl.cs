namespace ArtSpark.Demo.Utilities;

public static class ArtInstituteImageUrl
{
    public static string Build(string? imageId, string size = "843,", string format = "jpg")
    {
        if (string.IsNullOrWhiteSpace(imageId))
        {
            return string.Empty;
        }

        return $"/api/media/images/iiif/{Uri.EscapeDataString(imageId)}/full/{Uri.EscapeDataString(size)}/0/default.{Uri.EscapeDataString(format)}";
    }
}
