using CSX.DotNet.Common.FileCompression;
using System.Threading;
using System.Threading.Tasks;

namespace CSX.DotNet.Modules.FFMpeg.Provisioning.Features.Decompression;

public interface IDecompressionService
{
    Task<string> DecompressAsync(
        string archivePath,
        string decompressionPath = "",
        ExtractionOverwriteMode overwriteMode = ExtractionOverwriteMode.Abort,
        bool deleteZipFile = true,
        CancellationToken ctoken = default);
}