namespace Just.Core;

public static class Base64Url
{
    [Pure] public static Guid DecodeGuid(ReadOnlySpan<char> value)
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(value.Length, 22);

        Span<byte> guidBytes = stackalloc byte[16];
        Span<char> chars = stackalloc char[24];
        value.CopyTo(chars);
        for (int i = 0; i < value.Length; i++)
        {
            switch (value[i])
            {
                case '-': chars[i] = '+'; continue;
                case '_': chars[i] = '/'; continue;
                default: continue;
            }
        }
        chars[^2..].Fill('=');
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
        var outputBytes = charlen / 4;
        ArgumentOutOfRangeException.ThrowIfLessThan(output.Length, outputBytes);
        Span<char> chars = stackalloc char[charlen];

        value.CopyTo(chars);
        for (int i = 0; i < value.Length; i++)
        {
            switch (value[i])
            {
                case '-': chars[i] = '+'; continue;
                case '_': chars[i] = '/'; continue;
                default: continue;
            }
        }
        chars[^padding..].Fill('=');

        if (!Convert.TryFromBase64Chars(chars, output, out outputBytes))
            throw new FormatException("Invalid Base64 string.");

        return outputBytes;
    }
    
    [Pure] public static string Encode(in Guid id)
    {
        Span<byte> guidBytes = stackalloc byte[16];
        id.TryWriteBytes(guidBytes);
        Span<char> chars = stackalloc char[24];
        Convert.TryToBase64Chars(guidBytes, chars, out int _);

        for (int i = 0; i < chars.Length - 2; i++)
        {
            switch (chars[i])
            {
                case '+': chars[i] = '-'; continue;
                case '/': chars[i] = '_'; continue;
                default: continue;
            }
        }

        return new string(chars[..^2]);
    }

    [Pure] public static string Encode(ReadOnlySpan<byte> input)
    {
        if (input.IsEmpty) return string.Empty;

        int outLength = 8 * ((input.Length + 5) / 6);
        Span<char> output = stackalloc char[outLength];

        int strlen = Encode(input, output);
        return new string(output[..strlen]);
    }

    [Pure] public static int Encode(ReadOnlySpan<byte> input, Span<char> output)
    {
        if (input.IsEmpty) return 0;

        var charlen = 8 * ((input.Length + 5) / 6);
        Span<char> chars = stackalloc char[charlen];
        Convert.TryToBase64Chars(input, chars, out int charsWritten);

        int i;
        for (i = 0; i < charsWritten; i++)
        {
            switch (chars[i])
            {
                case '+': chars[i] = '-'; continue;
                case '/': chars[i] = '_'; continue;
                case '=': goto exitLoop;
                default: continue;
            }
        }
        exitLoop:
        chars[..i].CopyTo(output);

        return i;
    }
}
