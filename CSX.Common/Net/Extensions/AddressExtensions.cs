using System.Net;
using System.Net.Sockets;

namespace CSX.Common.Net.Extensions;

public static class AddressExtensions
{
    /// <summary>
    /// Returns a string representation of the IP address,
    /// along with an alternative representation if applicable,
    /// e.g., IPv4 mapped to IPv6 or vice versa.
    /// </summary>
    public static string[] ToStringVariants(this IPAddress ip)
    {
        string ipStr = ip.ToString();
        string? alt = ip switch
        {
            // Provide mapped IPv6 version
            { AddressFamily: AddressFamily.InterNetwork } => ip.MapToIPv6().ToString(),
            // Provide IPv4 version for mapped IPv6
            { IsIPv4MappedToIPv6: true } => ip.MapToIPv4().ToString(),
            _ => null
        };

        return alt is not null && alt != ipStr
            ? [ipStr, alt]
            : [ipStr];
    }
}