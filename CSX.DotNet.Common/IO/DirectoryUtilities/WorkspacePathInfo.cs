namespace CSX.DotNet.Common.IO.DirectoryUtilities;

public readonly record struct WorkspacePathInfo(
    string AbsolutePath,
    string RelativePath,
    string RootPath,
    bool IsWorkspaceRoot
)
{
    public bool IsValid => !string.IsNullOrEmpty(AbsolutePath);

    public bool IsInsideWorkspace => !IsWorkspaceRoot;
}