using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace XenoServe.Features.AssetUpload.DTOs;

public sealed class AssetUploadRequest
{
    // Data

    [Required]
    public IFormFile Data { get; set; } = default!;

    // Metadata

    public string PreferredFilename { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;

    // Extra

    public string ExtraData { get; set; } = string.Empty;
    public string PageUrl { get; set; } = string.Empty;
    public string DataUrl { get; set; } = string.Empty;
}