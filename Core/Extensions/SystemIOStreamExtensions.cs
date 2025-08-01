namespace Just.Core.Extensions;

/// <summary>
/// Provides extension methods for <see cref="Stream"/> to fully populate buffers.
/// </summary>
public static class SystemIOStreamExtensions
{
    /// <summary>
    /// Reads data from the stream until the specified section of the buffer is filled.
    /// </summary>
    /// <param name="stream">The stream to read from</param>
    /// <param name="buffer">The buffer to populate</param>
    /// <param name="offset">The starting offset in the buffer</param>
    /// <param name="length">The number of bytes to read</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="stream"/> is null</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="offset"/> or <paramref name="length"/> is invalid</exception>
    /// <exception cref="EndOfStreamException">Thrown if the stream ends before filling the buffer</exception>
    /// <exception cref="IOException">Thrown for I/O errors during reading</exception>
    public static void Populate(this Stream stream, byte[] buffer, int offset, int length)
        => stream.Populate(buffer.AsSpan(offset, length));
    /// <summary>
    /// Reads data from the stream until the entire buffer is filled.
    /// </summary>
    /// <param name="stream">The stream to read from</param>
    /// <param name="buffer">The buffer to populate</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="stream"/> is null</exception>
    /// <exception cref="EndOfStreamException">Thrown if the stream ends before filling the buffer</exception>
    /// <exception cref="IOException">Thrown for I/O errors during reading</exception>
    public static void Populate(this Stream stream, byte[] buffer)
        => stream.Populate(buffer.AsSpan());
    /// <summary>
    /// Reads data from the stream until the specified span is filled.
    /// </summary>
    /// <param name="stream">The stream to read from</param>
    /// <param name="buffer">The span to populate</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="stream"/> is null</exception>
    /// <exception cref="EndOfStreamException">Thrown if the stream ends before filling the buffer</exception>
    /// <exception cref="IOException">Thrown for I/O errors during reading</exception>
    public static void Populate(this Stream stream, Span<byte> buffer)
    {
        ArgumentNullException.ThrowIfNull(stream);

        while (!buffer.IsEmpty)
        {
            var bytesRead = stream.Read(buffer);

            if (bytesRead == 0)
            {
                throw new EndOfStreamException();
            }

            buffer = buffer[bytesRead..];
        }
    }

    /// <summary>
    /// Asynchronously reads data from the stream until the entire buffer is filled.
    /// </summary>
    /// <param name="stream">The stream to read from</param>
    /// <param name="buffer">The buffer to populate</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A ValueTask representing the asynchronous operation</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="stream"/> is null</exception>
    /// <exception cref="EndOfStreamException">Thrown if the stream ends before filling the buffer</exception>
    /// <exception cref="OperationCanceledException">Thrown if canceled via cancellation token</exception>
    /// <exception cref="IOException">Thrown for I/O errors during reading</exception>
    public static ValueTask PopulateAsync(this Stream stream, byte[] buffer, CancellationToken cancellationToken = default)
        => stream.PopulateAsync(buffer.AsMemory(), cancellationToken);
    /// <summary>
    /// Asynchronously reads data from the stream until the specified section of the buffer is filled.
    /// </summary>
    /// <param name="stream">The stream to read from</param>
    /// <param name="buffer">The buffer to populate</param>
    /// <param name="offset">The starting offset in the buffer</param>
    /// <param name="length">The number of bytes to read</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A ValueTask representing the asynchronous operation</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="stream"/> is null</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="offset"/> or <paramref name="length"/> is invalid</exception>
    /// <exception cref="EndOfStreamException">Thrown if the stream ends before filling the buffer</exception>
    /// <exception cref="OperationCanceledException">Thrown if canceled via cancellation token</exception>
    /// <exception cref="IOException">Thrown for I/O errors during reading</exception>
    public static ValueTask PopulateAsync(this Stream stream, byte[] buffer, int offset, int length, CancellationToken cancellationToken = default)
        => stream.PopulateAsync(buffer.AsMemory(offset, length), cancellationToken);
    /// <summary>
    /// Asynchronously reads data from the stream until the specified memory region is filled.
    /// </summary>
    /// <param name="stream">The stream to read from</param>
    /// <param name="buffer">The memory region to populate</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A ValueTask representing the asynchronous operation</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="stream"/> is null</exception>
    /// <exception cref="EndOfStreamException">Thrown if the stream ends before filling the buffer</exception>
    /// <exception cref="OperationCanceledException">Thrown if canceled via cancellation token</exception>
    /// <exception cref="IOException">Thrown for I/O errors during reading</exception>
    public static async ValueTask PopulateAsync(this Stream stream, Memory<byte> buffer, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(stream);

        while (!buffer.IsEmpty)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var bytesRead = await stream.ReadAsync(buffer, cancellationToken).ConfigureAwait(false);

            if (bytesRead == 0)
            {
                throw new EndOfStreamException();
            }

            buffer = buffer[bytesRead..];
        }
    }
}
