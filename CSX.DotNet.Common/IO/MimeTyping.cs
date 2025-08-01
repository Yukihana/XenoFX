using HeyRed.Mime;

namespace CSX.DotNet.Common.IO;

public static partial class MimeTyping
{
    public static string GetExtension(string mimeType)
    {
        return MimeTypesMap.GetExtension(mimeType);
    }

    public static string GetMimeType(string extension)
    {
        return MimeTypesMap.GetMimeType(extension);
    }
}