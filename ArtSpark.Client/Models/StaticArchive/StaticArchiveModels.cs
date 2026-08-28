using System.Text.Json.Serialization;
using ArtSpark.Client.Models.Common;

namespace ArtSpark.Client.Models.StaticArchive;

/// <summary>
/// Represents a site in the static archive
/// </summary>
public class Site : BaseResource
{
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("web_url")]
    public string? WebUrl { get; set; }
}
