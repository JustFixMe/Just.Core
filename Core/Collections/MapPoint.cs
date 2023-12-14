namespace Just.Core.Collections;

public readonly struct MapPoint<T>(T value, int x, int y, IMap<T> map)
{
    public readonly T Value = value;
    public IMap<T> Map { get; } = map;
    public int X { get; } = x;
    public int Y { get; } = y;
}
