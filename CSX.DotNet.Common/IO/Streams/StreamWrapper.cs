using System.IO;

namespace CSX.DotNet.Common.IO.Streams;

/// <summary>
/// Allow attaching side data to the stream
/// </summary>
public sealed record StreamWrapper<TStream, TAttachment>(
    TStream Stream,
    TAttachment Attachment)
    : IStreamWithAttachment<TStream, TAttachment>
    where TStream : Stream
{
    // Explicit implementation for non-generic interface
    Stream IStreamWithAttachment.Stream => Stream;
    object IStreamWithAttachment.Attachment => Attachment!;
}