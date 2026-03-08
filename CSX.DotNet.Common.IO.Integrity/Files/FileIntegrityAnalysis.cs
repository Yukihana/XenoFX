using CSX.DotNet.Common.IO.Integrity.Strategies.CompositeHashing;
using CSX.DotNet.Common.IO.Integrity.Strategies.CrumbsSampling;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace CSX.DotNet.Common.IO.Integrity.Files;

public static partial class FileIntegrityAnalysis
{
    public static Task ReadHeuristicsAsync(
        string filePath,
        IFileHeuristicsSignature signature,
        CancellationToken ctoken = default)
    {
        ctoken.ThrowIfCancellationRequested();

        signature.Location = filePath;
        FileInfo fi = new(filePath);
        signature.Size = fi.Length;
        signature.ModifiedUtc = fi.LastWriteTimeUtc;
        signature.CreatedUtc = fi.CreationTimeUtc;

        return Task.CompletedTask;
    }

    public static async Task ReadContentPartialsAsync(
        string filePath,
        IFileContentSignature signature,
        CancellationToken ctoken = default)
    {
        ctoken.ThrowIfCancellationRequested();
        signature.Crumbs = await CrumbsSampler.FromFile(filePath, ctoken);
    }

    public static async Task GenerateHashesAsync(
        string filePath,
        IFileHashSignature signature,
        CancellationToken ctoken = default)
    {
        ctoken.ThrowIfCancellationRequested();

        var hasher = new MultiHasher().WithSHA256().WithBlake3();

        var results = await hasher.HashFileAsync(
            filePath: filePath,
            parallelHashing: true,
            ctoken: ctoken
            ).ConfigureAwait(false);

        signature.SHA256
            = results.TryGetValue("SHA256", out var sha256)
            ? sha256 : throw new InvalidDataException("Missing hash");
        signature.Blake3
            = results.TryGetValue("BLAKE3", out var blake3)
            ? blake3 : throw new InvalidDataException("Missing hash");
    }

    // Wrappers

    public static async Task<IFileHeuristicsSignature> CreateHeuristicsSignatureAsync(
        string filePath,
        CancellationToken ctoken = default)
    {
        FileSignature signature = new();
        await ReadHeuristicsAsync(filePath, signature, ctoken);
        return signature;
    }

    public static async Task<IFileContentSignature> CreateContentSignatureAsync(
        string filePath,
        CancellationToken ctoken = default)
    {
        FileSignature signature = new();
        await ReadContentPartialsAsync(filePath, signature, ctoken);
        return signature;
    }

    public static async Task<IFileHashSignature> CreateHashSignatureAsync(
        string filePath,
        CancellationToken ctoken = default)
    {
        FileSignature signature = new();
        await GenerateHashesAsync(filePath, signature, ctoken);
        return signature;
    }

    // Full

    public static async Task<FileSignature> GenerateFullReportAsync(
        string filePath,
        bool generateHeuristicSignature = true,
        bool generateContentSignature = true,
        bool generateHashSignature = true,
        CancellationToken ctoken = default)
    {
        ctoken.ThrowIfCancellationRequested();

        FileSignature signature = new();

        if (generateHeuristicSignature)
            await ReadHeuristicsAsync(filePath, signature, ctoken);

        if (generateContentSignature)
            await ReadContentPartialsAsync(filePath, signature, ctoken);

        if (generateHashSignature)
            await GenerateHashesAsync(filePath, signature, ctoken);

        return signature;
    }
}