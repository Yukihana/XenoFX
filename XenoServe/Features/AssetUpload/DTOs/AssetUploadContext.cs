using XenoServe.Data;

namespace XenoServe.Features.AssetUpload.DTOs;

public class AssetUploadContext
{
    public RequestAuthorization Authorization { get; }
    public AssetUploadRequest Request { get; }

    // Lifecycle

    public AssetUploadContext(
        RequestAuthorization authorization,
        AssetUploadRequest request)
    {
        Authorization = authorization;
        Request = request;
    }
}