using System.ComponentModel.DataAnnotations;

namespace XenoServe.Features.AssetDelivery.DTOs;

public class AssetDeliveryRequest
{
    [Required]
    public string Id { get; set; } = string.Empty;

    public string? Type { get; set; } = null;

    public string? TranscodeType { get; set; } = null;
}