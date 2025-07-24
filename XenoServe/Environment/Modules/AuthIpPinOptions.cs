using CSX.DotNet.Modules.AuthIpPin.Environment.Configuration;
using XenoServe.Environment.Configuration;

namespace XenoServe.Environment.Modules;

public class AuthIpPinOptions : IAuthIpPinOptions
{
    // Defaults

    public const string DefaultDataDirectoryName = "AuthIpPin";

    // Infrastructure

    private readonly XenoServeConfiguration _config;

    // Lifecycle

    private AuthIpPinOptions(
        XenoServeConfiguration config)
    {
        _config = config;
        DataDirectory = _config.GetModuleDirectory(DefaultDataDirectoryName);
    }

    // Factory

    public static AuthIpPinOptions CreateFrom(
        XenoServeConfiguration config)
        => new(config);

    // Module

    public string DataDirectory { get; }

    // Shared
}