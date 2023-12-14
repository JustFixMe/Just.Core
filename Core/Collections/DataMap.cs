using System.Numerics;
using System.Runtime.InteropServices;
using Just.Core.Extensions;

namespace Just.Core.Collections;

public class DataMap<T> : Map<T>, IDataMap<T>, ICloneable
    where T : IComparisonOperators<T, T, bool>, IEqualityOperators<T, T, bool>
{
    public static int ElementSize { get; } = Marshal.SizeOf<T>();

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

public static class DataMap
{
    public const int HeaderSize = sizeof(Int64) + 2 * sizeof(UInt32);
    public struct Header
    {
        public Int64 BodySize;
        public UInt32 Width;
        public UInt32 Height;
    }

    public static async ValueTask WriteToStreamAsync<T>(this DataMap<T> map, Stream stream, CancellationToken cancellationToken = default)
        where T : unmanaged, IComparisonOperators<T, T, bool>, IEqualityOperators<T, T, bool>
    {
        var head = new Header
        {
            BodySize = DataMap<T>.ElementSize * map.Width * map.Height,
            Width = (uint)map.Width,
            Height = (uint)map.Height
        };

        var headBytes = MemoryMarshal.AsBytes(MemoryMarshal.CreateReadOnlySpan(ref head, 1))
            .ToArray();
        var bodyBytes = MemoryMarshal.AsBytes(map.AsSpan())
            .ToArray();

        await stream.WriteAsync(headBytes, cancellationToken);
        await stream.WriteAsync(bodyBytes, cancellationToken);
    }
    public static async ValueTask<DataMap<T>> ReadFromStreamAsync<T>(Stream stream, CancellationToken cancellationToken = default)
        where T : unmanaged, IComparisonOperators<T, T, bool>, IEqualityOperators<T, T, bool>
    {
        byte[] headBytes = new byte[HeaderSize];
        await stream.PopulateAsync(headBytes, cancellationToken);
        var head = MemoryMarshal.Read<Header>(headBytes);

        var bodySize = DataMap<T>.ElementSize * head.Width * head.Height;
        if (bodySize != head.BodySize) throw new InvalidOperationException("Can not read DataMap. Element size mismatch.");

        byte[] bodyBytes = new byte[bodySize];
        await stream.PopulateAsync(bodyBytes, cancellationToken);

        T[] body = MemoryMarshal.Cast<byte, T>(bodyBytes).ToArray();
        return new DataMap<T>((int)head.Width, (int)head.Height, body);
    }


    public static void WriteToStream<T>(this DataMap<T> map, Stream stream)
        where T : unmanaged, IComparisonOperators<T, T, bool>, IEqualityOperators<T, T, bool>
    {
        var head = new Header
        {
            BodySize = DataMap<T>.ElementSize * map.Width * map.Height,
            Width = (uint)map.Width,
            Height = (uint)map.Height
        };

        var headBytes = MemoryMarshal.AsBytes(MemoryMarshal.CreateReadOnlySpan(ref head, 1));
        var bodyBytes = MemoryMarshal.AsBytes(map.AsSpan());

        stream.Write(headBytes);
        stream.Write(bodyBytes);
    }
    public static DataMap<T> ReadFromStream<T>(Stream stream)
        where T : unmanaged, IComparisonOperators<T, T, bool>, IEqualityOperators<T, T, bool>
    {
        Header head = default;
        var headSpan = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref head, 1));
        stream.Populate(headSpan);

        var bodySize = DataMap<T>.ElementSize * head.Width * head.Height;
        if (bodySize != head.BodySize) throw new InvalidOperationException("Can not read DataMap. Element size mismatch.");

        T[] body = new T[bodySize];
        var bodySpan = MemoryMarshal.AsBytes(body.AsSpan());

        stream.Populate(bodySpan);
        
        return new DataMap<T>((int)head.Width, (int)head.Height, body);
    }
}
