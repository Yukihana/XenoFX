namespace XenoFx.Services.Processing.AssetIngestion.DTOs;

public class CachedAssetIndex : AssetIndexBase
{
    public string CacheFilePath { get; set; } = string.Empty;
    public string DeterminedFileName { get; set; } = string.Empty;
    public string FinalFullPath { get; set; } = string.Empty;
}