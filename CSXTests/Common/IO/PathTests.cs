using CSXTests.TestsDataProvider;

namespace CSXTests.Common.IO;

public class PathTests
{
    [Fact]
    public void TestMatcherVsWin32()
    {
        string[] fakePaths = FilePathGenerator.GenerateRandomWindowsPaths(1000);
    }
}