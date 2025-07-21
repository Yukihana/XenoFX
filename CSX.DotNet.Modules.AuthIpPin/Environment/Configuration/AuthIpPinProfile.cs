using System.IO;

namespace CSX.DotNet.Modules.AuthIpPin.Environment.Configuration;

public class AuthIpPinProfile
{
    // Defaults

    public const string DefaultFilename = "authIpPin.json";

    public static string GetFilePath(string dataDirectory) => Path.Combine(
        dataDirectory,
        DefaultFilename);

    // Module specific : Database

    public string DatabaseDirectoryName { get; set; } = "Database";

    public string AuthDatabasePath { get; set; } = "Auth.sqlite";

    // Required for db type specific actions
    public string AuthDatabaseType { get; set; } = "sqlite"; // Make this an enum
}