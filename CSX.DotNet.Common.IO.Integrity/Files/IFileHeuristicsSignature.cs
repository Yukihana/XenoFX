using System;

namespace CSX.DotNet.Common.IO.Integrity.Files;

public interface IFileHeuristicsSignature
{
    string? Location { get; set; }
    DateTimeOffset? CreatedUtc { get; set; }
    DateTimeOffset? ModifiedUtc { get; set; }
    long? Size { get; set; }
}