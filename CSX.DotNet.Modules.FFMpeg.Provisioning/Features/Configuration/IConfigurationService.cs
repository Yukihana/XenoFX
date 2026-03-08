namespace CSX.DotNet.Modules.FFMpeg.Provisioning.Features.Configuration;

public interface IConfigurationService
{
    string BinariesDirectory { get; }
    string SharedCacheDirectory { get; }
}