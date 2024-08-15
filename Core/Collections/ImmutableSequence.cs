using System.Collections;
using System.Collections.Immutable;

namespace Just.Core.Collections;

public class ImmutableSequence<T> :
    IEnumerable<T>,
    IReadOnlyList<T>,
    IEquatable<ImmutableSequence<T>>
{
    private static readonly int InitialHash = typeof(ImmutableSequence<T>).GetHashCode();
    private static readonly Func<T?, T?, bool> CompareItem = EqualityComparer<T>.Default.Equals;
    private readonly ImmutableList<T> _values;

    public ImmutableSequence(ImmutableList<T> values) => _values = values;
    public ImmutableSequence() : this(ImmutableArray<T>.Empty)
    {
    }
    public ImmutableSequence(IEnumerable<T> values)
    {
        _values = [..values];
    }
    public ImmutableSequence(ReadOnlySpan<T> values) : this(ImmutableList.Create(values))
    {
    }

    public bool IsEmpty => _values.IsEmpty;
    public int Count => _values.Count;
    public T this[int index] => _values[index];
    public T this[Index index] => _values[index];
    public ImmutableSequence<T> this[Range range]
    {
        get
        {
            var (offset, count) = range.GetOffsetAndLength(_values.Count);
            return ConstructNew(_values.GetRange(offset, count));
        }
    }

    protected virtual ImmutableSequence<T> ConstructNew(ImmutableList<T> values) => new(values);

    public ImmutableSequence<T> Add(T value) => ConstructNew([.._values, value]);
    public ImmutableSequence<T> AddFront(T value) => ConstructNew([value, .._values]);

    public ImmutableList<T>.Enumerator GetEnumerator() => _values.GetEnumerator();
    IEnumerator<T> IEnumerable<T>.GetEnumerator() => ((IEnumerable<T>)_values).GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_values).GetEnumerator();

    public override string ToString() => string.Join(Environment.NewLine, _values);

    public virtual bool Equals([NotNullWhen(true)] ImmutableSequence<T>? other)
    {
        if (ReferenceEquals(this, other))
        {
            return true;
        }

        if (_values.Count != other?._values.Count)
        {
            return false;
        }

        for (int i = 0; i < _values.Count; i++)
        {
            if (!CompareItem(_values[i], other._values[i]))
            {
                return false;
            }
        }

        return true;
    }

    public override bool Equals([NotNullWhen(true)] object? obj) => Equals(obj as ImmutableSequence<T>);
    public override int GetHashCode()
    {
        HashCode hash = new();
        hash.Add(InitialHash);

        foreach (var value in _values)
        {
            hash.Add(value);
        }

        return hash.ToHashCode();
    }

    public static bool operator ==(ImmutableSequence<T>? left, ImmutableSequence<T>? right) => left is null ? right is null : left.Equals(right);
    public static bool operator !=(ImmutableSequence<T>? left, ImmutableSequence<T>? right) => !(left == right);
}
