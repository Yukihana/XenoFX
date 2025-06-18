using CSX.Common.Data.Exceptions;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace XenoFx.Services.Abstraction.AssetAbstraction;

public partial class AssetAbstractionService
{
    public async Task<string> GetAssetFilePathAsync(string id, CancellationToken ctoken = default)
    {
        // Note:
        // Intended: AssetInfo object -> cross check presences -> return current asset file path
        // For now:
        // relative path -> full path, return if valid, else notify
        string fullPath = GetAssetFullPath(id);

        if (File.Exists(fullPath))
            return fullPath;

        // If the file does not exist, notify and throw an exception
        await NotifyOnAssetMissingAsync(fullPath, ctoken);
        throw new ResourceNotFoundException($"Asset file id:{id} missing at:{fullPath}");
    }
}