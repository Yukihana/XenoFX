using System;
using System.Collections;
using System.Collections.Generic;

namespace CSX.DotNet.Common.Data.Collections;

public class OrderedPriorityQueue<TElement, TPriority>
    : IEnumerable<TElement>
    where TPriority : notnull
{
    private readonly SortedDictionary<TPriority, Queue<TElement>> _dictionary;
    private readonly IComparer<TPriority> _comparer;

    // Properties

    public int Count { get; private set; } = 0;

    // Lifecycle

    public OrderedPriorityQueue()
        : this(null)
    { }

    public OrderedPriorityQueue(
        IComparer<TPriority>? comparer = null)
    {
        _comparer = comparer ?? Comparer<TPriority>.Default;
        _dictionary = new SortedDictionary<TPriority, Queue<TElement>>(_comparer);
    }

    public OrderedPriorityQueue(
        IEnumerable<(TElement Element, TPriority Priority)> items,
        IComparer<TPriority>? comparer = null)
        : this(comparer)
    {
        foreach (var (element, priority) in items)
            Enqueue(element, priority);
    }

    // Enqueue / Range

    public void Enqueue(
        TElement element,
        TPriority priority)
    {
        if (!_dictionary.TryGetValue(priority, out var queue))
        {
            queue = new Queue<TElement>();
            _dictionary.Add(priority, queue);
        }
        queue.Enqueue(element);
        Count++;
    }

    public void EnqueueRange(
        IEnumerable<TElement> elements,
        TPriority priority)
    {
        foreach (var element in elements)
            Enqueue(element, priority);
    }

    // Dequeue / Peek

    private TElement UnsafeGetNextInternal(
        bool remove)
    {
        if (Count == 0)
            throw new InvalidOperationException("The queue is empty.");

        // Get the first priority key
        using var enumerator = _dictionary.GetEnumerator();
        enumerator.MoveNext();
        var (firstKey, queue) = enumerator.Current;

        // Peek at the element
        var element = queue.Peek();

        // Remove if requested
        if (remove)
        {
            queue.Dequeue();
            if (queue.Count == 0)
                _dictionary.Remove(firstKey);
            Count--;
        }

        return element;
    }

    public TElement Dequeue()
        => UnsafeGetNextInternal(remove: true);

    public TElement Peek()
        => UnsafeGetNextInternal(remove: false);

    // TryDequeue / TryPeek

    private bool TryGetNextInternal(
        out TElement element,
        bool remove)
    {
        if (Count == 0)
        {
            element = default!;
            return false;
        }

        element = UnsafeGetNextInternal(remove);
        return true;
    }

    public bool TryDequeue(out TElement element)
        => TryGetNextInternal(out element, remove: true);

    public bool TryPeek(out TElement element)
        => TryGetNextInternal(out element, remove: false);

    // --- IEnumerable Support ---

    /// <summary>
    /// Not thread safe. Use for inspection purposes only.
    /// </summary>
    public IEnumerator<TElement> GetEnumerator()
    {
        foreach (var queue in _dictionary.Values)
        {
            foreach (var item in queue)
                yield return item;
        }
    }

    /// <summary>
    /// Not thread safe. Use for inspection purposes only.
    /// </summary>
    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();
}