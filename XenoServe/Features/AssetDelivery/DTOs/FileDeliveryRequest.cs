using System.ComponentModel.DataAnnotations;

namespace XenoServe.Features.AssetDelivery.DTOs;

public class FileDeliveryRequest
{
    [Required]
    public string Id { get; set; } = string.Empty;

    public string? Type { get; set; } = null;
}