using System.Collections;

namespace Just.Core.Collections;

public class Map<T> : IMap<T>
{
    internal readonly T[] _values;

    internal Map(int width, int height, T[] values)
    {
        Width = width;
        Height = height;
        _values = values;
    }
    public Map(ReadOnlySpan<T> values, int width, int height)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(width, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(height, 1);
        ArgumentOutOfRangeException.ThrowIfNotEqual(width * height, values.Length);

        Width = width;
        Height = height;
        _values = new T[values.Length];
        values.CopyTo(_values);
    }
    
    public Map(T[] values, int width, int height)
    {
        ArgumentNullException.ThrowIfNull(values);
        ArgumentOutOfRangeException.ThrowIfLessThan(width, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(height, 1);
        ArgumentOutOfRangeException.ThrowIfNotEqual(width * height, values.Length);

        Width = width;
        Height = height;
        _values = new T[Width * Height];
        values.CopyTo(_values.AsSpan());
    }
    public Map(T[,] values)
    {
        ArgumentNullException.ThrowIfNull(values);
        ArgumentOutOfRangeException.ThrowIfZero(values.Length);
        Width = values.GetLength(0);
        Height = values.GetLength(1);

        _values = new T[Width * Height];

        var data = _values.AsSpan();
        for (int y = 0; y < Height; y++)
        {
            int idy = y * Width;
            for (int x = 0; x < Width; x++)
            {
                int id = idy + x;
                data[id] = values[x, y];
            }
        }
    }

    [Pure] public ReadOnlySpan<T> AsSpan() => _values;
    [Pure] public ReadOnlyMemory<T> AsMemory() => _values;

    public int Width { get; }
    public int Height { get; }
    public int Count => _values.Length;

    [Pure] public ref readonly T this[int x, int y]
    {
        get
        {
            x = Math.Clamp(x, 0, Width);
            y = Math.Clamp(y, 0, Height);
            return ref _values[(y * Width) + x];
        }
    }
    [Pure] public ref readonly T this[double x, double y]
    {
        get
        {
            int xi = Math.Clamp((int)x, 0, Width - 1);
            int yi = Math.Clamp((int)y, 0, Height - 1);

            return ref _values[(yi * Width) + xi];
        }
    }

    [Pure] public MapPoint<T> Get(int x, int y)
    {
        x = Math.Clamp(x, 0, Width - 1);
        y = Math.Clamp(y, 0, Height - 1);
        return new(_values[(y * Width) + x], x, y, this);
    }
    [Pure] public MapPoint<T> Get(double x, double y)
    {
        int xi = Math.Clamp((int)x, 0, Width - 1);
        int yi = Math.Clamp((int)y, 0, Height - 1);

        return new(_values[(yi * Width) + xi], xi, yi, this);
    }

    [Pure] public T[,] ToArray()
    {
        T[,] arr = new T[Width, Height];

        var data = _values.AsSpan();
        for (int y = 0; y < Height; y++)
        {
            int idy = y * Width;
            for (int x = 0; x < Width; x++)
            {
                int id = idy + x;
                arr[x, y] = data[id];
            }
        }

        return arr;
    }
    public Enumerator GetEnumerator() => new(this);
    IEnumerator<MapPoint<T>> IEnumerable<MapPoint<T>>.GetEnumerator() => new Enumerator(this);
    IEnumerator IEnumerable.GetEnumerator() => new Enumerator(this);

    public struct Enumerator : IEnumerator<MapPoint<T>>, IEnumerator
    {
        private readonly Map<T> _map;
        private int _index;
        private MapPoint<T> _current;

        internal Enumerator(Map<T> map)
        {
            _map = map;
            _index = 0;
            _current = default;
        }

        public readonly void Dispose() { }
        public bool MoveNext()
        {
            if ((uint)_index < (uint)_map.Count)
            {
                _current = new(
                    _map._values[_index],
                    _index % _map.Width,
                    _index / _map.Width,
                    _map);
                _index++;
                return true;
            }

            _index = _map.Count + 1;
            _current = default;
            return false;
        }

        public readonly MapPoint<T> Current => _current!;
        readonly object? IEnumerator.Current
        {
            get
            {
                if (_index == 0 || _index == _map.Count + 1)
                {
                    throw new InvalidOperationException();
                }
                return Current;
            }
        }
        void IEnumerator.Reset()
        {
            _index = 0;
            _current = default;
        }
    }
}
