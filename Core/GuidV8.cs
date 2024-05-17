using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace Just.Core;

public enum GuidV8Entropy { Strong, Weak }

public static class GuidV8
{
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Guid NewGuid(GuidV8Entropy entropy = GuidV8Entropy.Strong) => NewGuid(DateTime.UtcNow, entropy);

    [Pure]
    public static Guid NewGuid(DateTime dateTime, GuidV8Entropy entropy = GuidV8Entropy.Strong)
    {
        var timestamp = dateTime.ToBinary() & 0x3FFF_FFFF_FFFF_FFFF;

        Span<byte> ts = stackalloc byte[8];
        MemoryMarshal.Write(ts, timestamp);

        Span<byte> bytes = stackalloc byte[16];

        ts[4..].CopyTo(bytes[..4]);
        ts[2..4].CopyTo(bytes[4..6]);
        ts[..2].CopyTo(bytes[6..8]);

        bytes[7] = (byte)((bytes[7] & 0x0f) | 0x80);

        if (entropy == GuidV8Entropy.Strong)
        {
            RandomNumberGenerator.Fill(bytes[8..]);
        }
        else
        {
            Random.Shared.NextBytes(bytes[8..]);
        }

        return new Guid(bytes);
    }
}
