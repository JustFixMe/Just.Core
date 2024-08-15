namespace Just.Core;

public static class Base32
{
    public const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
    public const char Padding = '=';

    [Pure]
    public static string Encode(ReadOnlySpan<byte> input)
    {
        if (input.Length == 0)
        {
            return string.Empty;
        }

        int outLength = 8 * ((input.Length + 4) / 5);
        Span<char> output = stackalloc char[outLength];

        _ = Encode(input, output);

        return new string(output);
    }
    
    [Pure]
    public static int Encode(ReadOnlySpan<byte> input, Span<char> output)
    {
        if (input.IsEmpty) return 0;

        int i = 0;
        ReadOnlySpan<char> alphabet = Alphabet;
        for (int offset = 0; offset < input.Length;)
        {
            int numCharsToOutput = GetNextGroup(input, ref offset, out byte a, out byte b, out byte c, out byte d, out byte e, out byte f, out byte g, out byte h);

            output[i++] = (numCharsToOutput > 0) ? alphabet[a] : Padding;
            output[i++] = (numCharsToOutput > 1) ? alphabet[b] : Padding;
            output[i++] = (numCharsToOutput > 2) ? alphabet[c] : Padding;
            output[i++] = (numCharsToOutput > 3) ? alphabet[d] : Padding;
            output[i++] = (numCharsToOutput > 4) ? alphabet[e] : Padding;
            output[i++] = (numCharsToOutput > 5) ? alphabet[f] : Padding;
            output[i++] = (numCharsToOutput > 6) ? alphabet[g] : Padding;
            output[i++] = (numCharsToOutput > 7) ? alphabet[h] : Padding;
        }
        return i;
    }

    [Pure]
    public static byte[] Decode(ReadOnlySpan<char> input)
    {
        if (input.IsEmpty) return [];
        
        Span<byte> output = stackalloc byte[5 * input.Length / 8];
        
        var size = Decode(input, output);

        return output[..size].ToArray();
    }

    [Pure]
    public static int Decode(ReadOnlySpan<char> input, Span<byte> output)
    {
        input = input.TrimEnd(Padding);
        Span<char> inputspan = stackalloc char[input.Length];
        input.ToUpperInvariant(inputspan);

        int bitIndex = 0;
        int inputIndex = 0;
        int outputBits = 0;
        int outputIndex = 0;
        int bitPos;
        int outBitPos;
        int bits;

        ReadOnlySpan<char> alphabet = Alphabet;
        while (inputIndex < input.Length)
        {
            var byteIndex = alphabet.IndexOf(inputspan[inputIndex]);
            if (byteIndex < 0)
            {
                throw new FormatException("Provided string contains invalid characters.");
            }
            
            bitPos = 5 - bitIndex;
            outBitPos = 8 - outputBits;
            bits = bitPos < outBitPos ? bitPos : outBitPos;

            output[outputIndex] <<= bits;
            output[outputIndex] |= (byte)(byteIndex >> (bitPos - bits));

            outputBits += bits;
            if (outputBits >= 8)
            {
                outputIndex++;
                outputBits = 0;
            }

            bitIndex += bits;
            if (bitIndex >= 5)
            {
                inputIndex++;
                bitIndex = 0;
            }
            else if (inputIndex == input.Length -1) break;
        }

        return outputIndex + (outputBits + 7) / 8;
    }

    
    // returns the number of bytes that were output
    [Pure]
    private static int GetNextGroup(ReadOnlySpan<byte> input, ref int offset, out byte a, out byte b, out byte c, out byte d, out byte e, out byte f, out byte g, out byte h)
    {
        var retVal = (input.Length - offset) switch
        {
            1 => 2,
            2 => 4,
            3 => 5,
            4 => 7,
            _ => 8,
        };
        uint b1 = (offset < input.Length) ? input[offset++] : 0U;
        uint b2 = (offset < input.Length) ? input[offset++] : 0U;
        uint b3 = (offset < input.Length) ? input[offset++] : 0U;
        uint b4 = (offset < input.Length) ? input[offset++] : 0U;
        uint b5 = (offset < input.Length) ? input[offset++] : 0U;

        a = (byte)(b1 >> 3);
        b = (byte)(((b1 & 0x07) << 2) | (b2 >> 6));
        c = (byte)((b2 >> 1) & 0x1f);
        d = (byte)(((b2 & 0x01) << 4) | (b3 >> 4));
        e = (byte)(((b3 & 0x0f) << 1) | (b4 >> 7));
        f = (byte)((b4 >> 2) & 0x1f);
        g = (byte)(((b4 & 0x3) << 3) | (b5 >> 5));
        h = (byte)(b5 & 0x1f);

        return retVal;
    }
}
