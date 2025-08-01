using CSX.DotNet.Common.Security.Integrity;

namespace XenoFx.Services.Processing.AssetIngestion.DTOs;

public class FileAnalysisReport
{
    public bool IsExecutable { get; set; } = false;
    public string DeterminedExtension { get; set; } = string.Empty;
    public string DeterminedMimeType { get; set; } = string.Empty; // Probably not needed
    public IntegrityDigestV1? IntegrityInfo { get; set; } = null;
}