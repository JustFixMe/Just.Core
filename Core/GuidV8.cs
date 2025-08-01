using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace Just.Core;

public static class GuidV8
{
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Guid NewGuid(RngEntropy entropy = RngEntropy.Strong) => NewGuid(DateTime.UtcNow, entropy);

    [Pure]
    public static Guid NewGuid(DateTime dateTime, RngEntropy entropy = RngEntropy.Strong)
    {
        var epoch = dateTime.Subtract(DateTime.UnixEpoch);
        var timestamp = epoch.Ticks / (TimeSpan.TicksPerMillisecond / 10);

        Span<byte> ts = stackalloc byte[8];
        MemoryMarshal.Write(ts, timestamp);

        Span<byte> bytes = stackalloc byte[16];

        ts[0..2].CopyTo(bytes[4..6]);
        ts[2..6].CopyTo(bytes[..4]);

        if (entropy == RngEntropy.Strong)
        {
            RandomNumberGenerator.Fill(bytes[6..]);
        }
        else
        {
            Random.Shared.NextBytes(bytes[6..]);
        }

        bytes[7] = (byte)((bytes[7] & 0x0F) | 0x80);

        return new Guid(bytes);
    }
}
