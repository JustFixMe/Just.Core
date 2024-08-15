namespace Just.Core.Extensions;

public static class SystemIOStreamExtensions
{
    public static void Populate(this Stream stream, byte[] buffer, int offset, int length)
        => stream.Populate(buffer.AsSpan(offset, length));
    public static void Populate(this Stream stream, byte[] buffer)
        => stream.Populate(buffer.AsSpan());
    public static void Populate(this Stream stream, Span<byte> buffer)
    {
        while (buffer.Length > 0)
        {
            var readed = stream.Read(buffer);

            if (readed == 0)
            {
                throw new EndOfStreamException();
            }

            buffer = buffer[readed..];
        }
    }

    public static async ValueTask PopulateAsync(this Stream stream, byte[] buffer, CancellationToken cancellationToken = default)
        => await stream.PopulateAsync(buffer.AsMemory(), cancellationToken);
    public static async ValueTask PopulateAsync(this Stream stream, byte[] buffer, int offset, int length, CancellationToken cancellationToken = default)
        => await stream.PopulateAsync(buffer.AsMemory(offset, length), cancellationToken);
    public static async ValueTask PopulateAsync(this Stream stream, Memory<byte> buffer, CancellationToken cancellationToken = default)
    {
        while (buffer.Length > 0)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var readed = await stream.ReadAsync(buffer, cancellationToken);

            if (readed == 0)
            {
                throw new EndOfStreamException();
            }

            buffer = buffer[readed..];
        }
    }
}
