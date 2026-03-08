using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CSX.DotNet.Common.IO.Integrity.Files;

public partial class FileSignatureBuilder
{
    // Infrastructure

    private readonly FileSignatureBuilderOptions _options;
    private readonly ILogger<FileSignatureBuilder>? _logger;

    // Data

    private readonly string _filePath;
    private readonly FileSignature _fileSignature = new();

    // Lifecycle

    public FileSignatureBuilder(
        string filePath,
        FileSignatureBuilderOptions? options = null,
        ILogger<FileSignatureBuilder>? logger = null)
    {
        _filePath = filePath;
        _options = options ?? new();
        _logger = logger;
    }

    // API

    public async Task<FileSignatureBuilder> ReadHeuristicsAsync(
        CancellationToken ctoken = default)
    {
        ctoken.ThrowIfCancellationRequested();

        try
        {
            await FileIntegrityAnalysis.ReadHeuristicsAsync(
                filePath: _filePath,
                signature: _fileSignature,
                ctoken: ctoken);

            return this;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error reading heuristics for file: {FilePath}", _filePath);
            throw;
        }
    }

    public async Task<FileSignatureBuilder> ReadContentPartialsAsync(
        CancellationToken ctoken = default)
    {
        ctoken.ThrowIfCancellationRequested();

        try
        {
            await FileIntegrityAnalysis.ReadContentPartialsAsync(
                filePath: _filePath,
                signature: _fileSignature,
                ctoken: ctoken);

            return this;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error reading content partials for file: {FilePath}", _filePath);
            throw;
        }
    }

    public async Task<FileSignatureBuilder> GenerateHashesAsync(
        CancellationToken ctoken = default)
    {
        ctoken.ThrowIfCancellationRequested();

        try
        {
            await FileIntegrityAnalysis.GenerateHashesAsync(
                filePath: _filePath,
                signature: _fileSignature,
                ctoken: ctoken);

            return this;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error generating hashes for file: {FilePath}", _filePath);
            throw;
        }
    }

    public FileSignature ToFileSignature()
        => _fileSignature;
}