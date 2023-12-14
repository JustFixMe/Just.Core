using System.Numerics;

namespace Just.Core.Collections;

public class DataMap<T> : Map<T>, IDataMap<T>, ICloneable
    where T : IComparisonOperators<T, T, bool>, IEqualityOperators<T, T, bool>
{
    internal MapPoint<T> MinCache = default;
    internal MapPoint<T> MaxCache = default;

    internal DataMap(int width, int height, T[] values)
        : base(width, height, values)
    {
    }

    public DataMap(ReadOnlySpan<T> values, int width, int height)
        : base(values, width, height)
    {
    }

    public DataMap(T[] values, int width, int height)
        : base(values, width, height)
    {
    }

    public DataMap(T[,] values)
        : base(values)
    {
    }

    [Pure] public MapPoint<T> Min
    {
        get
        {
            if (MinCache.Map is null) FindMinMax();
            return MinCache;
        }
    }

    [Pure] public MapPoint<T> Max
    {
        get
        {
            if (MaxCache.Map is null) FindMinMax();
            return MaxCache;
        }
    }

    private void FindMinMax()
    {
        var data = _values.AsSpan();

        T min = data[0];
        T max = data[0];
        int minId = 0;
        int maxId = 0;
        
        for (int i = 0; i < data.Length; i++)
        {
            if (data[i] < min)
            {
                min = data[i];
                minId = i;
                continue;
            }

            if (data[i] > max)
            {
                max = data[i];
                maxId = i;
            }
        }

        MinCache = new(min, minId % Width, minId / Width, this);
        MaxCache = new(max, maxId % Width, maxId / Width, this);
    }

    [Pure] public DataMap<T> Clone()
    {
        var clone = new DataMap<T>(Width, Height, [.. _values]);
        clone.MinCache = new(clone._values[(MinCache.Y * Width) + MinCache.X], MinCache.X, MinCache.Y, clone);
        clone.MaxCache = new(clone._values[(MaxCache.Y * Width) + MaxCache.X], MaxCache.X, MaxCache.Y, clone);
        return clone;
    }

    object ICloneable.Clone() => Clone();
}
