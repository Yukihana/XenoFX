using System;
using System.Text.Json;

namespace CSX.DotNet.Common.Extensions.Instancing;

public static partial class ObjectCopyExtensions
{
    /// <summary>
    /// Makes a deep copy using the Json Serializer
    /// </summary>
    /// <typeparam name="T">The object type.</typeparam>
    /// <param name="source">The object to be copied.</param>
    /// <returns>A truly decoupled deep copy.</returns>
    /// <exception cref="NullReferenceException">Returned when the copy operation fails.</exception>
    public static T MakeDecoupledCopy<T>(this T source)
        => JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(source))
        ?? throw new NullReferenceException();
}