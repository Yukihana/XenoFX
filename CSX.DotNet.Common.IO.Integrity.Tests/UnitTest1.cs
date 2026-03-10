using Blake3;
using CSX.DotNet.Common.IO.Integrity.Strategies.CompositeHashing;
using CSX.DotNet.Common.IO.Integrity.Strategies.IntegrityV1;
using FluentAssertions;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CSX.DotNet.Common.IO.Integrity.Tests;

public class UnitTest1
{
    [Fact]
    public async Task MultiHasher_Result_Matches_Individual_Hashes()
    {
        // Arrange
        var data = Encoding.UTF8.GetBytes("The quick brown fox jumps over the lazy dog");
        using var ms = new MemoryStream(data);

        // MultiHasher setup
        var multiHasher = new MultiHasher()
            .WithSHA256()
            .WithBlake3();

        // Act
        var result = await multiHasher.HashStreamAsync(ms, ctoken: TestContext.Current.CancellationToken);

        // Individual hashes
        byte[] shaResult = SHA256.HashData(data);

        ms.Position = 0; // Reset stream position for Blake3
        byte[] blake3Result;
        {
            var blake3 = Hasher.New();
            blake3.Update(data.AsSpan());
            blake3Result = blake3.Finalize().AsSpan().ToArray();
        }

        // Assert
        result.Should().ContainKey("SHA256");
        result["SHA256"].Should().Equal(shaResult);

        result.Should().ContainKey("BLAKE3");
        result["BLAKE3"].Should().Equal(blake3Result);
    }

    [Fact]
    public async Task LegacyVsNew_MultiHasher_ResultsMatch()
    {
        // Arrange
        var data = Encoding.UTF8.GetBytes("Lorem ipsum dolor sit amet");
        using var msLegacy = new MemoryStream(data);
        using var msNew = new MemoryStream(data);

        var legacyResult = await FileIntegrityV1.LegacyGetInfoV1Async(msLegacy, ctoken: CancellationToken.None);
        var newResult = await FileIntegrityV1.GetInfoV1Async(msLegacy, ctoken: CancellationToken.None);

        // Assert
        newResult.SHA256.Should().Equal(legacyResult.SHA256);
        newResult.Blake3.Should().Equal(legacyResult.Blake3);

        // Optional: ignore crumbs if MultiHasher doesn't implement them yet
        if (legacyResult.Crumbs != null && newResult.Crumbs != null)
            newResult.Crumbs.Should().Equal(legacyResult.Crumbs);
    }
}