using System.IO;

namespace XenoFx.Services.Api.AssetUpload.Models;

public sealed class AssetUploadRequest(Stream stream)
{
    public Stream Data { get; set; } = stream;
    public string PageUrl { get; set; } = string.Empty;
    public string DataUrl { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
}