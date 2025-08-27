using CSX.DotNet.Common.IO.DirectoryUtilities;
using CSX.DotNet.Common.IO.Storage;
using System.Threading;
using System.Threading.Tasks;

namespace CSX.DotNet.Modules.FFMpeg.Provisioning.Features.Download;

public interface IDownloadService
{
    Task<string> DownloadToWorkspaceAsync(
        string url,
        IWorkspace workspace,
        string relativePath = "",
        string preferredFileName = "",
        ConflictResolution resolution = ConflictResolution.None,
        CancellationToken ctoken = default);
}