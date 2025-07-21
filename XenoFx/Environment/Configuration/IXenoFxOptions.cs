namespace XenoFx.Environment.Configuration;

public interface IXenoFxOptions
{
    // Module's data directory

    string DataDirectory { get; }

    // Shared parameters

    string AssetsDirectory { get; }

    string SharedCacheDirectory { get; }
}