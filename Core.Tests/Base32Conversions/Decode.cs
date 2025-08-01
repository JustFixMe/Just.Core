namespace Just.Core.Tests.Base32Conversions;

public class Decode
{
    [Theory]
    [InlineData(15243)]
    [InlineData(812010)]
    [InlineData(97331334)]
    [InlineData(20354)]
    public void WhenEncodedToString_ShouldBeDecodedToTheSameByteArray(int seed)
    {
        var rng = new Random(seed);

        for (int i = 1; i <= 512; i++)
        {
            var testBytes = new byte[i];
            rng.NextBytes(testBytes);

            var resultString = Base32.Encode(testBytes);
            var resultBytes = Base32.Decode(resultString);

            resultBytes.Should().BeEquivalentTo(testBytes);
        }
    }

    [Theory]
    [InlineData("FG4M3ZQM3TVWDMBUP5L7N7V3JS7KBM2E", new byte[] { 0x29, 0xb8, 0xcd, 0xe6, 0x0c, 0xdc, 0xeb, 0x61, 0xb0, 0x34, 0x7f, 0x57, 0xf6, 0xfe, 0xbb, 0x4c, 0xbe, 0xa0, 0xb3, 0x44, })]
    [InlineData("WXYEOQUZULMCY6ZQTDOLTRUZZMKQ====", new byte[] { 0xb5, 0xf0, 0x47, 0x42, 0x99, 0xa2, 0xd8, 0x2c, 0x7b, 0x30, 0x98, 0xdc, 0xb9, 0xc6, 0x99, 0xcb, 0x15, })]
    [InlineData("2IO2HTALCXZWCBD2", new byte[] { 0xd2, 0x1d, 0xa3, 0xcc, 0x0b, 0x15, 0xf3, 0x61, 0x04, 0x7a, })]
    [InlineData("ZFXJMF5N", new byte[] { 0b11001001, 0b01101110, 0b10010110, 0b00010111, 0b10101101, })]
    [InlineData("CPIKTMY=", new byte[] { 0b00010011, 0b11010000, 0b10101001, 0b10110011, })]
    [InlineData("JVNJA===", new byte[] { 0b01001101, 0b01011010, 0b10010000, })]
    [InlineData("74OQ====", new byte[] { 0b11111111, 0b00011101, })]
    public void WhenCalledWithValidString_ShouldReturnValidByteArray(string str, byte[] expected)
    {
        var actualBytesArray = Base32.Decode(str);
        actualBytesArray.Should().Equal(expected);
    }

    [Theory]
    [InlineData("ZFXJMF5N====", new byte[] { 0b11001001, 0b01101110, 0b10010110, 0b00010111, 0b10101101, })]
    [InlineData("CPIKTMY=====", new byte[] { 0b00010011, 0b11010000, 0b10101001, 0b10110011, })]
    [InlineData("JVNJA=======", new byte[] { 0b01001101, 0b01011010, 0b10010000, })]
    public void WhenCalledWithValidStringThatEndsWithPaddingSign_ShouldReturnValidByteArray(string testString, byte[] expected)
    {
        var actualBytesArray = Base32.Decode(testString);
        actualBytesArray.Should().Equal(expected);
    }

    [Theory]
    [InlineData("ZFXJMF5N", new byte[] { 0b11001001, 0b01101110, 0b10010110, 0b00010111, 0b10101101, })]
    [InlineData("CPIKTMY", new byte[] { 0b00010011, 0b11010000, 0b10101001, 0b10110011, })]
    [InlineData("JVNJA", new byte[] { 0b01001101, 0b01011010, 0b10010000, })]
    public void WhenCalledWithValidStringWithoutPaddingSign_ShouldReturnValidByteArray(string testString, byte[] expected)
    {
        var actualBytesArray = Base32.Decode(testString);
        actualBytesArray.Should().Equal(expected);
    }

    [Theory]
    [InlineData("  ")]
    [InlineData("hg2515i3215")]
    [InlineData("hg712)21")]
    [InlineData("hg712f 21")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1806:Do not ignore method results", Justification = "Test case")]
    public void WhenCalledWithNotValidString_ShouldThrowFormatException(string testString)
    {
        Action action = () => Base32.Decode(testString);
        action.Should().Throw<FormatException>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void WhenCalledWithNullString_ShouldReturnEmptyArray(string? testString)
    {
        Base32.Decode(testString).Should().BeEmpty();
    }
}
