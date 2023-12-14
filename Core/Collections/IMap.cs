namespace Just.Core.Collections;

public interface IMap<T> : IReadOnlyCollection<MapPoint<T>>
{
    int Width { get; }
    int Height { get; }
    ref readonly T this[int x, int y] { get; }
    ref readonly T this[double x, double y] { get; }
    MapPoint<T> Get(int x, int y);
    MapPoint<T> Get(double x, double y);
    T[,] ToArray();
}
