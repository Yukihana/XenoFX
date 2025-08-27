using CSX.DotNet.Common.Platform;

namespace CSXTests.Common.Platform;

public class PlatformTagTests
{
    [Fact]
    public void GetCurrent_OsArch()
    {
        // Example format: only OS and arch
        string format = "{os}_{arch}";
        string tag = PlatformTag.GetCurrent(format);
        string expected = $"{SystemInformation.OS.GetTag()}_{SystemInformation.Architecture}";
        Assert.Equal(expected, tag, StringComparer.OrdinalIgnoreCase);
    }

    [Fact]
    public void GetCurrent_OsArchVer()
    {
        string format = "{os}_{arch}_{ver}";
        string tag = PlatformTag.GetCurrent(format);

        string expected = $"{SystemInformation.OS.GetTag()}_{SystemInformation.Architecture}_{SystemInformation.Version}";
        Assert.Equal(expected, tag, StringComparer.OrdinalIgnoreCase);
    }
}