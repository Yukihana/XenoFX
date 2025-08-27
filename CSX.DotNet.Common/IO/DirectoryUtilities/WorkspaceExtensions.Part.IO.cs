using CSX.DotNet.Common.IO.Storage;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace CSX.DotNet.Common.IO.DirectoryUtilities;

public static partial class WorkspaceExtensions
{
    public static async Task<string> SaveAsync(
        this IWorkspace workspace,
        Stream stream,
        string relativePath,
        bool overwrite = false,
        int bufferSize = 81920,
        CancellationToken ctoken = default)
    {
        return await stream.SaveToWorkspaceAsync(
            workspace,
            relativePath,
            overwrite,
            bufferSize,
            ctoken);
    }

    public static async Task<string> DownloadAsync(
        this IWorkspace workspace,
        string url,
        string relativePath,
        string preferredFileName = "",
        ConflictResolution resolution = ConflictResolution.None,
        CancellationToken ctoken = default)
    {
        return await DownloadUtility.ToWorkspaceAsync(
            url: url,
            workspace: workspace,
            preferredFileName: preferredFileName,
            relativePath: relativePath,
            resolution: resolution,
            ctoken: ctoken);
    }
}