using System.Security.Cryptography;

namespace Just.Core;

public static class GuidV8
{
    private const long TicksPrecision = TimeSpan.TicksPerMillisecond / 10;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Guid NewGuid(RngEntropy entropy = RngEntropy.Strong) => NewGuid(DateTime.UtcNow, entropy);

    public static Guid NewGuid(DateTime dateTime, RngEntropy entropy = RngEntropy.Strong)
    {
        var epoch = dateTime.Subtract(DateTime.UnixEpoch);
        var timestamp = epoch.Ticks / TicksPrecision;

        uint tsHigh = (uint)((timestamp >> 16) & 0xFFFFFFFF);
        ushort tsLow = (ushort)(timestamp & 0x0000FFFF);

        Span<byte> bytes = stackalloc byte[10];

        if (entropy == RngEntropy.Strong)
        {
            RandomNumberGenerator.Fill(bytes);
        }
        else
        {
            Random.Shared.NextBytes(bytes);
        }

        bytes[0] = (byte)((bytes[0] & 0x0F) | 0x80); // Version 8
        bytes[2] = (byte)((bytes[2] & 0x1F) | 0x80); // Variant 0b1000

        ushort version = (ushort)((bytes[0] << 8) | bytes[1]);

        return new Guid(
            tsHigh,
            tsLow,
            version,
            bytes[2], bytes[3], bytes[4], bytes[5], bytes[6], bytes[7], bytes[8], bytes[9]);
    }
}
