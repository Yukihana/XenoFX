namespace XenoFx.Services.Api.AssetSearch.Contracts;

public class AssetNextploreQuery
{
    // Concatenated search parameters

    public string Keywords { get; set; } = string.Empty;

    // Playback state

    public int PlaybackIndex { get; set; } = 0;
    public string OriginalId { get; set; } = string.Empty;
    public string CurrentId { get; set; } = string.Empty;

    // Extra data for randomization (e.g., IP address, user ID, etc.)

    public string PartialSeed { get; set; } = string.Empty;
}