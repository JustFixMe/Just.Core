namespace Just.Core.Tests.Base64UrlConversions;

public class Decode
{
    [Theory]
    [InlineData(72121)]
    [InlineData(554121)]
    [InlineData(100454567)]
    [InlineData(3210589)]
    public void WhenEncodedToString_ShouldBeDecodedToTheSameByteArray(int seed)
    {
        var rng = new Random(seed);

        for (int i = 1; i <= 512; i++)
        {
            var testBytes = new byte[i];
            rng.NextBytes(testBytes);

            var resultString = Base64Url.Encode(testBytes);
            var resultBytes = Base64Url.Decode(resultString);

            resultBytes.Should().BeEquivalentTo(testBytes);
        }
    }

    [Theory]
    [InlineData("5QrdUxDUVkCAEGw8pvLsEw", "53dd0ae5-d410-4056-8010-6c3ca6f2ec13")]
    [InlineData("6nE2uKQ4_0ar9kpmybgkdw", "b83671ea-38a4-46ff-abf6-4a66c9b82477")]
    [InlineData("PyD6zwDqXkGbS1HPsp41wQ", "cffa203f-ea00-415e-9b4b-51cfb29e35c1")]
    [InlineData("AdOlPOh3wEe9PlyQgTMt2g", "3ca5d301-77e8-47c0-bd3e-5c9081332dda")]
    [InlineData("0elO0Lr-UkWwTarBeY6HRA", "d04ee9d1-feba-4552-b04d-aac1798e8744")]
    public void WhenCalled_ShouldReturnValidGuid(string testString, string expectedStr)
    {
        var result = Base64Url.DecodeGuid(testString);
        var expected = Guid.Parse(expectedStr);
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("IA", new byte[]{ 0x20, })]
    [InlineData("Ag", new byte[]{ 0x02, })]
    [InlineData("ELg", new byte[]{ 0x10, 0xb8, })]
    [InlineData("Vv0", new byte[]{ 0x56, 0xfd, })]
    [InlineData("aVLO", new byte[]{ 0x69, 0x52, 0xce, })]
    [InlineData("Ww2w", new byte[]{ 0x5b, 0x0d, 0xb0, })]
    [InlineData("UKO0cR-OjLiM", new byte[]{ 0x50, 0xa3, 0xb4, 0x71, 0x1f, 0x8e, 0x8c, 0xb8, 0x8c, })]
    [InlineData("hOb_nnjJRirORuTzYA", new byte[]{ 0x84, 0xe6, 0xff, 0x9e, 0x78, 0xc9, 0x46, 0x2a, 0xce, 0x46, 0xe4, 0xf3, 0x60, })]
    public void WhenCalled_ShouldReturnValidBytes(string testString, byte[] expected)
    {
        var result = Base64Url.Decode(testString);
        result.Should().BeEquivalentTo(expected);
    }

    [Theory]
    [InlineData("  ")]
    [InlineData("hg2&515i3215")]
    [InlineData("hg712)21")]
    [InlineData("hg712f 21")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1806:Do not ignore method results", Justification = "Test case")]
    public void WhenCalledWithInvalidString_ShouldThrowFormatException(string testString)
    {
        Action action = () => Base64Url.Decode(testString);
        action.Should().Throw<FormatException>();
    }

    [Theory]
    [InlineData("5QrdUxDUV CAEGw8pvLsEw")]
    [InlineData("6nE2uKQ4$0ar9kpmybgkdw")]
    [InlineData("PyD6zwDqXkG*S1HPsp41wQ")]
    [InlineData("!dOlPOh3wEe9PlyQgTMt2g")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1806:Do not ignore method results", Justification = "Test case")]
    public void WhenCalledWithInvalidGuidString_ShouldThrowFormatException(string testString)
    {
        Action action = () => Base64Url.DecodeGuid(testString);
        action.Should().Throw<FormatException>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void WhenCalledWithNullString_ShouldReturnEmptyArray(string testString)
    {
        Base64Url.Decode(testString).Should().BeEmpty();
    }
}
