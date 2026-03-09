using CSX.DotNet.Modules.FileIndexing.Environment;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace CSX.DotNet.Modules.FileIndexing.Tests;

public class UnitTest1
{
	private readonly ITestOutputHelper _output;

	public UnitTest1(
		ITestOutputHelper output)
	{ _output = output; }
}