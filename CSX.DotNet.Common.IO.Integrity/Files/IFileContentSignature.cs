namespace CSX.DotNet.Common.IO.Integrity.Files;

public interface IFileContentSignature
{
    byte[]? Crumbs { get; set; }
}