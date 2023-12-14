using System.Numerics;

namespace Just.Core.Collections;

public interface IDataMap<T> : IMap<T>, ICloneable
    where T : IComparisonOperators<T, T, bool>, IEqualityOperators<T, T, bool>
{
    MapPoint<T> Min { get; }
    MapPoint<T> Max { get; }
}
