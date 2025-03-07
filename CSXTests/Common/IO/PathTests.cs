using CSXTests.TestsDataProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSXTests.Common.IO;

public class PathTests
{
    [Fact]
    public void TestMatcherVsWin32()
    {
        string[] fakePaths = FilePathGenerator.GenerateRandomWindowsPaths(1000);
    }
}