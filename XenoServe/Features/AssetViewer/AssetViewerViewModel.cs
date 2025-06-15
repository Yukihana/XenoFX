using XenoServe.Data;

namespace XenoServe.Features.AssetViewer;

public class AssetViewerViewModel
{
    public ViewRenderType RenderType { get; set; } = ViewRenderType.Partial;
    public string SourceUrl { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string MimeType { get; set; } = string.Empty;
    public string Extension { get; set; } = string.Empty;
}