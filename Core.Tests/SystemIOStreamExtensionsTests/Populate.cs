using Just.Core.Extensions;

namespace Just.Core.Tests.SystemIOStreamExtensionsTests;

public class Populate
{
    [Theory]
    [InlineData(0, 0)]
    [InlineData(0, 1)]
    [InlineData(0, 3)]
    [InlineData(0, 5)]
    [InlineData(3, 0)]
    [InlineData(3, 1)]
    [InlineData(3, 5)]
    [InlineData(5, 0)]
    [InlineData(5, 1)]
    [InlineData(5, 5)]
    public void WhenCalled_ShouldPopulateSpecifiedRange(int offset, int length)
    {
        byte[] streamContent = [0x01, 0x02, 0x03, 0x04, 0x05,];
        using var stream = new MemoryStream(streamContent);
        var buffer = new byte[10];

        stream.Populate(buffer, offset, length);

        buffer.Skip(offset).Take(length).ShouldBe(streamContent.Take(length));
    }

    [Theory]
    [InlineData(new byte[]{ 0x01, 0x02, 0x03, 0x04, }, 4)]
    [InlineData(new byte[]{ 0x01, 0x02, 0x03, 0x04, 0x05, }, 4)]
    [InlineData(new byte[]{ 0x01, 0x02, 0x03, 0x04, 0x05, }, 5)]
    [InlineData(new byte[]{ 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, }, 5)]
    public void WhenStreamContainsSameOrGreaterAmmountOfItems_ShouldPopulateBuffer(byte[] streamContent, int bufferSize)
    {
        using var stream = new MemoryStream(streamContent);
        var buffer = new byte[bufferSize];

        stream.Populate(buffer);

        buffer.ShouldBe(streamContent.Take(bufferSize));
    }

    [Theory]
    [InlineData(new byte[]{ 0x01, 0x02, 0x03, 0x04, }, 5)]
    [InlineData(new byte[]{ 0x01, 0x02, 0x03, 0x04, 0x05, }, 6)]
    [InlineData(new byte[]{ 0x01, 0x02, 0x03, 0x04, 0x05, }, 10)]
    [InlineData(new byte[]{ 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, }, 9)]
    public void WhenStreamContainsLessItems_ShouldThrowEndOfStreamException(byte[] streamContent, int bufferSize)
    {
        using var stream = new MemoryStream(streamContent);
        var buffer = new byte[bufferSize];

        Action action = () => stream.Populate(buffer);

        action.ShouldThrow<EndOfStreamException>();
    }
}
