using System.ComponentModel.DataAnnotations;

namespace XenoFx.Database.Models;

public partial class TagRecord
{
    [Key]
    public string Id { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}