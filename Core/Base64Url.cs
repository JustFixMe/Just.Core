using System.Runtime.InteropServices;

namespace Just.Core;

public static class Base64Url
{
    private const char Padding = '=';

    [Pure] public static long DecodeLong(ReadOnlySpan<char> value)
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(value.Length, 11);

        Span<byte> longBytes = stackalloc byte[8];
        Span<char> chars = stackalloc char[12];

        value.CopyTo(chars);
        chars[^1] = Padding;

        ReplaceNonUrlChars(chars);

        if (!Convert.TryFromBase64Chars(chars, longBytes, out int _))
            throw new FormatException("Invalid Base64 string.");

        return MemoryMarshal.Read<long>(longBytes);
    }

    [Pure] public static Guid DecodeGuid(ReadOnlySpan<char> value)
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(value.Length, 22);

        Span<byte> guidBytes = stackalloc byte[16];
        Span<char> chars = stackalloc char[24];

        value.CopyTo(chars);
        chars[^2..].Fill(Padding);

        ReplaceNonUrlChars(chars);

        if (!Convert.TryFromBase64Chars(chars, guidBytes, out int _))
            throw new FormatException("Invalid Base64 string.");

        return new Guid(guidBytes);
    }

    [Pure] public static byte[] Decode(ReadOnlySpan<char> input)
    {
        if (input.IsEmpty) return [];

        Span<byte> output = stackalloc byte[3 * ((input.Length + 3) / 4)];

        var size = Decode(input, output);

        return output[..size].ToArray();
    }

    [Pure] public static int Decode(ReadOnlySpan<char> value, Span<byte> output)
    {
        var padding = (4 - (value.Length & 3)) & 3;
        var charlen = value.Length + padding;
        var outputBytes = 3 * (charlen / 4);
        ArgumentOutOfRangeException.ThrowIfLessThan(output.Length, outputBytes);
        Span<char> chars = stackalloc char[charlen];

        value.CopyTo(chars);
        chars[^padding..].Fill(Padding);

        ReplaceNonUrlChars(chars);

        if (!Convert.TryFromBase64Chars(chars, output, out outputBytes))
            throw new FormatException("Invalid Base64 string.");

        return outputBytes;
    }

    [Pure] public static string Encode(in long id)
    {
        Span<byte> longBytes = stackalloc byte[8];
        MemoryMarshal.Write(longBytes, id);

        Span<char> chars = stackalloc char[12];
        Convert.TryToBase64Chars(longBytes, chars, out int _);
        ReplaceUrlChars(chars[..^1]);

        return new string(chars[..^1]);
    }

    [Pure] public static string Encode(in Guid id)
    {
        Span<byte> guidBytes = stackalloc byte[16];
        id.TryWriteBytes(guidBytes);
        Span<char> chars = stackalloc char[24];
        Convert.TryToBase64Chars(guidBytes, chars, out int _);

        ReplaceUrlChars(chars[..^2]);

        return new string(chars[..^2]);
    }

    [Pure] public static string Encode(ReadOnlySpan<byte> input)
    {
        if (input.IsEmpty) return string.Empty;

        int outLength = 4 * ((input.Length + 2) / 3);
        Span<char> output = stackalloc char[outLength];

        int strlen = Encode(input, output);
        return new string(output[..strlen]);
    }

    [Pure] public static int Encode(ReadOnlySpan<byte> input, Span<char> output)
    {
        if (input.IsEmpty) return 0;

        var charlen = 4 * ((input.Length + 2) / 3);
        Span<char> chars = stackalloc char[charlen];
        Convert.TryToBase64Chars(input, chars, out int charsWritten);

        int i = ReplaceUrlChars(chars[..charsWritten]);
        chars[..i].CopyTo(output);

        return i;
    }

    private static int ReplaceUrlChars(Span<char> chars)
    {
        int i = 0;
        for (; i < chars.Length; i++)
        {
            switch (chars[i])
            {
                case '+': chars[i] = '-'; continue;
                case '/': chars[i] = '_'; continue;
                case Padding: goto break_loop;
                default: continue;
            }
        }
        break_loop:
        return i;
    }

    private static int ReplaceNonUrlChars(Span<char> chars)
    {
        int i = 0;
        for (; i < chars.Length; i++)
        {
            switch (chars[i])
            {
                case '-': chars[i] = '+'; continue;
                case '_': chars[i] = '/'; continue;
                case Padding: goto break_loop;
                default: continue;
            }
        }
        break_loop:
        return i;
    }
}
