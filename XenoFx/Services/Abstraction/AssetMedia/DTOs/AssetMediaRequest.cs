namespace XenoFx.Services.Abstraction.AssetMedia.DTOs;

public class AssetMediaRequest
{
    public string Id { get; set; } = string.Empty;

    public string? Type { get; set; } = null;

    public TranscodeOptions TranscodeOptions { get; set; } = new();
}