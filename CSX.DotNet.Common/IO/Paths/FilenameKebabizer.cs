using CSX.DotNet.Common.Data.Text.Sanitization;
using System;

namespace CSX.DotNet.Common.IO.Paths;

[Obsolete("Use CSX.Data.Text.Sanitization.FileNameSanitizer instead. This class will be removed.")]
public static partial class FilenameKebabizer
{
    public static string Sanitize(string input)
        => FileNameSanitizer.Sanitize(input);

    public static string FromUrl(string url)
        => FileNameSanitizer.FromUrl(url);

    public static string FromTitle(string title)
        => FileNameSanitizer.FromTitle(title);
}