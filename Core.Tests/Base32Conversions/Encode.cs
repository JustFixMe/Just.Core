 namespace Just.Core.Tests.Base32Conversions;

public class Encode
{
    [Theory]
    [InlineData("TQ======")]
    [InlineData("3X3A====")]
    [InlineData("426G6===")]
    [InlineData("C3V3Y===")]
    [InlineData("2JDBWKVU")]
    [InlineData("ELCQNL477Q======")]
    [InlineData("52Q2JBWZ36AQ====")]
    [InlineData("INANSVEXD4FQ====")]
    [InlineData("5HHUA5YXV5EEQ===")]
    [InlineData("K7QEZW5N7H4TGIQ=")]
    [InlineData("HVC7AUT2MZA646QS")]
    [InlineData("ZHMQCLPPTQMSDSFKMI======")]
    [InlineData("C4GQGXL5ZPMB5PIRAE======")]
    [InlineData("OD4LXSFOWCJCVRZG63OA====")]
    [InlineData("7XLZBFDEWHSU2ZZWE7XYK===")]
    [InlineData("DCIEHJ672Z3DSGAR762Z6===")]
    [InlineData("4OY6MNQ5R35Y3OK23U66CDI=")]
    [InlineData("GSHXGB5ORKNDSFLSU2YWALI=")]
    [InlineData("TMFSC64ZZNPQSNGCFIODS7TR")]
    [InlineData("ZTDUBU4QZFFMDJKBII334EIB")]
    public void WhenDecodedFromString_ShouldBeEncodedToTheSameString(string testString)
    {
        var resultBytes = Base32.Decode(testString);
        var resultString = Base32.Encode(resultBytes);

        resultString.Should().Be(testString);
    }

    [Theory]
    [InlineData("FG4M3ZQM3TVWDMBUP5L7N7V3JS7KBM2E", new byte[] { 0x29, 0xb8, 0xcd, 0xe6, 0x0c, 0xdc, 0xeb, 0x61, 0xb0, 0x34, 0x7f, 0x57, 0xf6, 0xfe, 0xbb, 0x4c, 0xbe, 0xa0, 0xb3, 0x44, })]
    [InlineData("WXYEOQUZULMCY6ZQTDOLTRUZZMKQ====", new byte[] { 0xb5, 0xf0, 0x47, 0x42, 0x99, 0xa2, 0xd8, 0x2c, 0x7b, 0x30, 0x98, 0xdc, 0xb9, 0xc6, 0x99, 0xcb, 0x15, })]
    [InlineData("2IO2HTALCXZWCBD2", new byte[] { 0xd2, 0x1d, 0xa3, 0xcc, 0x0b, 0x15, 0xf3, 0x61, 0x04, 0x7a, })]
    [InlineData("ZFXJMF5N", new byte[] { 0b11001001, 0b01101110, 0b10010110, 0b00010111, 0b10101101, })]
    [InlineData("CPIKTMY=", new byte[] { 0b00010011, 0b11010000, 0b10101001, 0b10110011, })]
    [InlineData("JVNJA===", new byte[] { 0b01001101, 0b01011010, 0b10010000,  })]
    [InlineData("74OQ====", new byte[] { 0b11111111, 0b00011101, })]
    public void WhenCalledWithNotEmptyByteArray_ShouldReturnValidString(string expected, byte[] testArray)
    {
        var str = Base32.Encode(testArray);
        str.Should().Be(expected);
    } 
    
    [Theory]
    [InlineData(new byte[] { })]
    [InlineData(null)]
    public void WhenCalledWithEmptyByteArray_ShouldReturnEmptyString(byte[] testArray)
    {
        var actualBase32 = Base32.Encode(testArray);
        actualBase32.Should().Be(string.Empty);
    }
}
