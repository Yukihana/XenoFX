using CSX.DotNet.Common.Data.Validations;
using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;

namespace CSX.DotNet.Common.Data.Collections;

public sealed class PooledArray<T> :
    IDisposable,
    IList<T>,
    IList,
    IReadOnlyList<T>
{
    // Resources

    public ArrayPool<T> Pool { get; }
    public T[] Buffer { get; } // Resource to be cleared at disposal
    public int Capacity { get; }

    private int _count;

    public int Count
    {
        get => _count;
        private set => _count = ResolverUtility.ValidateLength(
            length: value,
            maxLength: Capacity,
            errorMessage: "Cannot exceed buffer capacity.");
    }

    public bool ClearOnReturn { get; set; } = false; // Runtime modifiable

    // Lifecycle

    public PooledArray(
        int length,
        ArrayPool<T>? pool = null)
    {
        Pool = pool ?? ArrayPool<T>.Shared;
        Buffer = Pool.Rent(length);
        Capacity = Buffer.Length;

        Count = length.EnsureNotNegative(
            errorMessage: "Length cannot be negative.");
    }

    public PooledArray(
        ReadOnlySpan<T> span,
        ArrayPool<T>? pool = null)
        : this(span.Length, pool)
    {
        span.CopyTo(Buffer.AsSpan(0, Count));
    }

    public PooledArray(
        ArraySegment<T> segment,
        ArrayPool<T>? pool = null)
        : this(segment.AsSpan(), pool)
    { }

    public PooledArray(
        ArrayPool<T>? pool,
        params T[] items)
        : this(items.AsSpan(), pool)
    { }

    public void Dispose()
        => Pool.Return(Buffer, clearArray: ClearOnReturn);

    // Add-on

    public void Resize(int newSize)
        => Count = newSize; // Automatically invokes validation in the setter

    // Indexing API

    public T this[int index]
    {
        get => Buffer[ResolverUtility.ResolveIndex(index, Count)];
        set => Buffer[ResolverUtility.ResolveIndex(index, Count)] = value;
    }

    object? IList.this[int index]
    {
        get => this[index]!;
        set => this[index] = (T)value!;
    }

    // Optimizations : Span / Memory

    public ReadOnlySpan<T> AsSpan()
        => Buffer.AsSpan(0, Count);

    public ReadOnlySpan<T> AsSpan(int length) => Buffer.AsSpan(
        0, ResolverUtility.ValidateLength(length, Count));

    public ReadOnlySpan<T> AsSpan(int start, int length) => Buffer.AsSpan(
        ResolverUtility.ResolveIndex(start, Count),
        ResolverUtility.ValidateLength(length, Count));

    public ReadOnlyMemory<T> AsMemory()
        => Buffer.AsMemory(0, Count);

    public ReadOnlyMemory<T> AsMemory(int length) => Buffer.AsMemory(
        0, ResolverUtility.ValidateLength(length, Count));

    public ReadOnlyMemory<T> AsMemory(int start, int length) => Buffer.AsMemory(
        ResolverUtility.ResolveIndex(start, Count),
        ResolverUtility.ValidateLength(length, Count));

    // Optimizations : Logical copy

    public T[] ToArray()
        => Buffer[..Count];

    public List<T> ToList()
        => [.. Buffer[..Count]];

    // Implementation: IList<T>

    public int IndexOf(T item)
    {
        for (int i = 0; i < Count; i++)
        {
            if (EqualityComparer<T>.Default.Equals(Buffer[i], item))
                return i;
        }
        return -1;
    }

    public void Insert(int index, T item)
    {
        index = ResolverUtility.ValidateIndex(index, Count + 1);
        Count++; // validation happens in the setter

        // Shift items
        Buffer
            .AsSpan(index, Count - index - 1)
            .CopyTo(Buffer.AsSpan(index + 1));

        // Actual insert
        Buffer[index] = item;
    }

    public void RemoveAt(int index)
    {
        index = ResolverUtility.ValidateIndex(index, Count);

        // Shift items
        Buffer
            .AsSpan(index + 1, Count - index - 1)
            .CopyTo(Buffer.AsSpan(index));

        // Update count and clear the previously-last item
        Count--;
        Buffer[Count] = default!;
    }

    public void Add(T item)
    {
        Count++; // validation happens in the setter
        Buffer[Count - 1] = item;
    }

    public void Clear()
        => Count = 0;

    public bool Contains(T item)
        => IndexOf(item) >= 0;

    public void CopyTo(T[] array, int arrayIndex)
    {
        Buffer.AsSpan(0, Count)
            .CopyTo(array.AsSpan(arrayIndex));
    }

    public bool Remove(T item)
    {
        int idx = IndexOf(item);
        if (idx < 0) return false;
        RemoveAt(idx);
        return true;
    }

    public IEnumerator<T> GetEnumerator()
    {
        for (int i = 0; i < Count; i++)
            yield return Buffer[i];
    }

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    // Interface : IList (Non-generic)

    public int Add(object? value)
    {
        Add((T)value!);
        return Count - 1;
    }

    public bool Contains(object? value)
        => Contains((T)value!);

    public int IndexOf(object? value)
        => IndexOf((T)value!);

    public void Insert(int index, object? value)
        => Insert(index, (T)value!);

    public void Remove(object? value)
        => Remove((T)value!);

    public void CopyTo(Array array, int index)
    {
        // For wrong array types, use a more specific error message:
        if (array is not T[])
        {
            throw new ArgumentException(
                $"Array must be of type {typeof(T).Name}[]",
                nameof(array));
        }

        CopyTo((T[])array, index);
    }

    public bool IsReadOnly => false;
    public bool IsFixedSize => false;
    public bool IsSynchronized => false;
    public object SyncRoot => this;
}