using System.IO;

namespace CSX.DotNet.Modules.AuthIpPin.Environment.Configuration;

public class AuthIpPinConfiguration
{
    // Infrastructure

    private readonly AuthIpPinProfile _profile;
    private readonly IAuthIpPinOptions _options;

    // Lifecycle

    public AuthIpPinConfiguration(
        AuthIpPinProfile profile,
        IAuthIpPinOptions options)
    {
        _profile = profile;
        _options = options;

        // Snapshot and cache

        // Prepare shared runtime parameters
    }

    // Directory and Profile

    public string DataDirectory
        => _options.DataDirectory;

    public string GetProfilePath()
        => AuthIpPinProfile.GetFilePath(DataDirectory); // Not needed but keep anyway

    // Auth Database

    public string ProfilePath => Path.Combine(
        DataDirectory,
        AuthIpPinProfile.DefaultFilename);

    public string AuthDatabasePath => Path.Combine(
        _options.DataDirectory,
        _profile.DatabaseDirectoryName,
        _profile.AuthDatabasePath); // TODO Resolve, to account for full, or remote paths.

    public string AuthDatabaseType
        => _profile.AuthDatabaseType;

    // Parameters

    public bool NoAuth
        => _profile.NoAuth;
}