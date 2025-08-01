using System.IO;

namespace CSX.DotNet.Common.IO.Streams;

public interface IStreamWithAttachment
{
    Stream Stream { get; }
    object Attachment { get; }
}

public interface IStreamWithAttachment<TStream, TAttachment> : IStreamWithAttachment
    where TStream : Stream
{
    new TStream Stream { get; }
    new TAttachment Attachment { get; }
}