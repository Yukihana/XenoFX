using System;
using System.Collections.Generic;

namespace CSX.Common.Data.Collections;

public class ArrayWrapper<T>
{
    private readonly T[] _items;

    public ArrayWrapper(int length)
    {
        _items = new T[length];
    }

    public ArrayWrapper(T[] source)
    {
        ArgumentNullException.ThrowIfNull(source);

        _items = new T[source.Length];
        Array.Copy(source, _items, source.Length);
    }

    public T this[int index]
    {
        get => _items[index];
        set => _items[index] = value;
    }

    public int Length => _items.Length;

    // Implicit conversion if you need interop
    public static implicit operator T[](ArrayWrapper<T> arr) => arr._items;

    public Span<T> AsSpan() => _items.AsSpan();

    public T[] ToArray() => [.. _items];

    public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)_items).GetEnumerator();

    public ReadOnlySpan<T> AsReadOnlySpan() => _items.AsSpan();
}