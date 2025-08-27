namespace CSX.DotNet.Common.Abstractions;

public interface IHasAttachment<T>
{
    T? Attachment { get; set; }
}