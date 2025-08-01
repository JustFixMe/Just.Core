using System.Security.Cryptography;

namespace Just.Core;

/// <summary>
/// Generates time-based sequential IDs with entropy
/// <para>ID Structure (64 bits):</para>
/// <list type="bullet">
/// <item><description>[1 bit] Always 0 (positive signed longs)</description></item>
/// <item><description>[41 bits] Milliseconds since epoch (covers ~69 years)</description></item>
/// <item><description>[8 bits] Sequence counter (0-255 per millisecond)</description></item>
/// <item><description>[14 bits] Random entropy (0-16383)</description></item>
/// </list>
/// </summary>
/// <remarks>
/// Important behaviors:
/// <list type="bullet">
/// <item><description>Guarantees monotonic ordering within same millisecond</description></item>
/// <item><description>Throws <see cref="InvalidOperationException"/> on backward time jumps</description></item>
/// <item><description>Sequence resets when timestamp advances</description></item>
/// <item><description>Thread-safe through locking</description></item>
/// </list>
/// </remarks>
public sealed class SeqId(DateTime epoch)
{
    private const int TimestampBits = 41;
    private const int TimestampShift = 63 - TimestampBits;
    private const long TimestampMask = 0x000001FF_FFFFFFFF;

    private const int SeqBits = 8;
    private const int SeqShift = TimestampShift - SeqBits;
    private const long SeqMask = 0x00000000_000000FF;

    private const int RandExclusiveUpper = 1 << SeqShift;

    /// <summary>
    /// Default epoch (2025-01-01 UTC) used for <see cref="Default"/> instance
    /// </summary>
    public static DateTime DefaultEpoch { get; } = new(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    /// <summary>
    /// Default instance using <see cref="DefaultEpoch"/>
    /// </summary>
    public static SeqId Default { get; } = new(DefaultEpoch);

    /// <summary>
    /// Generates ID using default instance and current UTC time
    /// </summary>
    /// <param name="entropy">Entropy quality (default: Strong)</param>
    /// <returns>64-bit sequential ID with random component</returns>
    /// <exception cref="IndexOutOfRangeException">
    /// Thrown if more than 255 IDs generated in 1ms
    /// </exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long NextId(RngEntropy entropy = RngEntropy.Strong) => Default.Next(DateTime.UtcNow, entropy);

#if NET9_0_OR_GREATER
    private readonly Lock _lock = new();
#else
    private readonly object _lock = new();
#endif

    private readonly DateTime _epoch = epoch;
    private int _seqId = 0;
    private long _lastTimestamp = -1L;

    /// <summary>
    /// Generates next ID using current UTC time
    /// </summary>
    /// <param name="entropy">Entropy quality (default: Strong)</param>
    /// <returns>64-bit sequential ID with random component</returns>
    /// <exception cref="IndexOutOfRangeException">
    /// Thrown if more than 255 IDs generated in 1ms
    /// </exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long Next(RngEntropy entropy = RngEntropy.Strong) => Next(DateTime.UtcNow, entropy);

    /// <summary>
    /// Generates next ID with explicit timestamp
    /// </summary>
    /// <param name="dateTime">Timestamp basis for ID generation</param>
    /// <param name="entropy">Entropy quality (default: Strong)</param>
    /// <returns>64-bit sequential ID with random component</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if <paramref name="dateTime"/> is earlier than last used timestamp
    /// </exception>
    /// <exception cref="IndexOutOfRangeException">
    /// Thrown if more than 255 IDs generated in 1ms
    /// </exception>
    public long Next(DateTime dateTime, RngEntropy entropy = RngEntropy.Strong)
    {
        var epoch = dateTime.Subtract(_epoch);
        var timestamp = ((epoch.Ticks / TimeSpan.TicksPerMillisecond) & TimestampMask) << TimestampShift;

        long currentSeq;
        lock (_lock)
        {
            if (timestamp > _lastTimestamp)
            {
                _lastTimestamp = timestamp;
                _seqId = 0;
            }
            else if (timestamp < _lastTimestamp)
            {
                throw new InvalidOperationException("Refused to create new SeqId. Last timestamp is in the future.");
            }

            if (_seqId == SeqMask)
            {
                throw new IndexOutOfRangeException("Refused to create new SeqId. Sequence exhausted.");
            }

            currentSeq = ((_seqId++) & SeqMask) << SeqShift;
        }

        long currentRand = entropy == RngEntropy.Strong
            ? RandomNumberGenerator.GetInt32(RandExclusiveUpper)
            : Random.Shared.Next(RandExclusiveUpper);

        return timestamp | currentSeq | currentRand;
    }
}
