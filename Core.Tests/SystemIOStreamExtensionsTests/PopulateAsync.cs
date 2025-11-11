using Just.Core.Extensions;

namespace Just.Core.Tests.SystemIOStreamExtensionsTests;

public class PopulateAsync
{
    [Fact]
    public async Task WhenCancellationRequested_ShouldThrowOperationCanceledException()
    {
        using var cts = new CancellationTokenSource();
        byte[] streamContent = [0x01, 0x02, 0x03, 0x04, 0x05,];
        using var stream = new MemoryStream(streamContent);
        var buffer = new byte[5];

        Func<Task> action = async () => await stream.PopulateAsync(buffer, cts.Token);
        cts.Cancel();

        await action.ShouldThrowAsync<OperationCanceledException>();
    }

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
    public async Task WhenCalled_ShouldPopulateSpecifiedRange(int offset, int length)
    {
        using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(50));
        byte[] streamContent = [0x01, 0x02, 0x03, 0x04, 0x05,];
        using var stream = new MemoryStream(streamContent);
        var buffer = new byte[10];

        await stream.PopulateAsync(buffer, offset, length, cts.Token);

        buffer.Skip(offset).Take(length).ShouldBe(streamContent.Take(length));
    }

    [Theory]
    [InlineData(new byte[]{ 0x01, 0x02, 0x03, 0x04, }, 4)]
    [InlineData(new byte[]{ 0x01, 0x02, 0x03, 0x04, 0x05, }, 4)]
    [InlineData(new byte[]{ 0x01, 0x02, 0x03, 0x04, 0x05, }, 5)]
    [InlineData(new byte[]{ 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, }, 5)]
    public async Task WhenStreamContainsSameOrGreaterAmmountOfItems_ShouldPopulateBuffer(byte[] streamContent, int bufferSize)
    {
        using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(50));
        using var stream = new MemoryStream(streamContent);
        var buffer = new byte[bufferSize];

        await stream.PopulateAsync(buffer, cts.Token);

        buffer.ShouldBe(streamContent.Take(bufferSize));
    }

    [Theory]
    [InlineData(new byte[]{ 0x01, 0x02, 0x03, 0x04, }, 5)]
    [InlineData(new byte[]{ 0x01, 0x02, 0x03, 0x04, 0x05, }, 6)]
    [InlineData(new byte[]{ 0x01, 0x02, 0x03, 0x04, 0x05, }, 10)]
    [InlineData(new byte[]{ 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, }, 9)]
    public async Task WhenStreamContainsLessItems_ShouldThrowEndOfStreamException(byte[] streamContent, int bufferSize)
    {
        using var stream = new MemoryStream(streamContent);
        var buffer = new byte[bufferSize];

        Func<Task> action = async () => await stream.PopulateAsync(buffer);

        await action.ShouldThrowAsync<EndOfStreamException>();
    }
}
