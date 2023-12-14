namespace Just.Core.Tests.Base64UrlConversions;

public class Encode
{
    [Theory]
    [InlineData("5QrdUxDUVkCAEGw8pvLsEw", "53dd0ae5-d410-4056-8010-6c3ca6f2ec13")]
    [InlineData("6nE2uKQ4_0ar9kpmybgkdw", "b83671ea-38a4-46ff-abf6-4a66c9b82477")]
    [InlineData("PyD6zwDqXkGbS1HPsp41wQ", "cffa203f-ea00-415e-9b4b-51cfb29e35c1")]
    [InlineData("AdOlPOh3wEe9PlyQgTMt2g", "3ca5d301-77e8-47c0-bd3e-5c9081332dda")]
    [InlineData("0elO0Lr-UkWwTarBeY6HRA", "d04ee9d1-feba-4552-b04d-aac1798e8744")]
    public void WhenCalledWithGuid_ShouldReturnValidString(string expected, string testGuidString)
    {
        var testGuid = Guid.Parse(testGuidString);
        var result = Base64Url.Encode(testGuid);
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
    public void WhenCalled_ShouldReturnValidString(string expected, byte[] testBytes)
    {
        var result = Base64Url.Encode(testBytes);
        result.Should().Be(expected);
    }
}
