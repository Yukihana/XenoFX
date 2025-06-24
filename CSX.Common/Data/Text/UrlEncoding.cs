using System;

namespace CSX.Common.Data.Text;

public static class UrlEncoding
{
    public static string Base64UrlEncode(byte[] bytes) => Convert
        .ToBase64String(bytes)
        .TrimEnd('=')
        .Replace('+', '-')
        .Replace('/', '_');

    public static byte[] Base64UrlDecode(string input)
    {
        string base64 = input
            .Replace('-', '+')
            .Replace('_', '/');
        switch (input.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }
        return Convert.FromBase64String(base64);
    }
}