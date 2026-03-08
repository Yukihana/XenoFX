namespace CSX.DotNet.Common.IO.Integrity.Files;

public interface IFileHashSignature
{
    byte[]? SHA256 { get; set; }
    byte[]? Blake3 { get; set; }
}