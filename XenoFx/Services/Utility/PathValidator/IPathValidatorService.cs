using System;
using System.Diagnostics.CodeAnalysis;

namespace XenoFx.Services.Utility.PathValidator;

public interface IPathValidatorService
{
    bool TryTruncateAssetPath(string path, [NotNullWhen(true)] out string? relativePath);
}