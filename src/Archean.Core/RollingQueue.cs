using System.Collections;

namespace Archean.Core;

/// <summary>
/// A fixed-size queue, which drops items in the order that they were added (FIFO) when the number of items exceeds the capacity.
/// </summary>
/// <typeparam name="T"></typeparam>
public class RollingQueue<T> : IReadOnlyCollection<T>
{
    private readonly LinkedList<T> _list = [];

    /// <summary>
    /// The maximum number of items allowed in the collection before items are dropped.
    /// </summary>
    public int Capacity { get; }

    /// <summary>
    /// The direction in which the items are arranged when iterating over the collection.
    /// </summary>
    public RollingQueueDirection Direction { get; }

    public RollingQueue(int capacity) : this(capacity, RollingQueueDirection.FirstToLast)
    {
    }

    public RollingQueue(int capacity, RollingQueueDirection direction)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(capacity, nameof(capacity));

        if (!Enum.IsDefined(direction))
        {
            throw new ArgumentException("Invalid queue direction", nameof(direction));
        }

        Capacity = capacity;
        Direction = direction;
    }

    /// <summary>
    /// Add <paramref name="item"/> to the list.
    /// </summary>
    /// <param name="item"></param>
    public void Add(T item)
    {
        while (_list.Count >= Capacity)
        {
            if (Direction == RollingQueueDirection.FirstToLast)
            {
                _list.RemoveFirst();
            }
            else
            {
                _list.RemoveLast();
            }
        }

        if (Direction == RollingQueueDirection.FirstToLast)
        {
            _list.AddLast(item);
        }
        else
        {
            _list.AddFirst(item);
        }
    }

    /// <summary>
    /// Add <paramref name="items"/> to the list.
    /// </summary>
    /// <param name="items"></param>
    public void AddRange(IEnumerable<T> items)
    {
        foreach (T item in items)
        {
            Add(item);
        }
    }

    /// <summary>
    /// Removes all items from the list.
    /// </summary>
    public void Clear()
    {
        _list.Clear();
    }

    /// <summary>
    /// Returns the number of items current in the list.
    /// </summary>
    public int Count => _list.Count;

    public IEnumerator<T> GetEnumerator()
    {
        return _list.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public enum RollingQueueDirection
{
    FirstToLast,
    LastToFirst
}
