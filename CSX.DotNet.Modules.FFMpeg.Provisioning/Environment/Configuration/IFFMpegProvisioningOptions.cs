namespace CSX.DotNet.Modules.FFMpeg.Provisioning.Environment.Configuration;

public interface IFFMpegProvisioningOptions
{
    string DataDirectory { get; }
    string BinariesDirectory { get; }
    string SharedCacheDirectory { get; }
}