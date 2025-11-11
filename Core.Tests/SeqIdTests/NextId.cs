
namespace Just.Core.Tests.SeqIdTests;

public class NextId
{
    private static readonly DateTime TestEpoch = new(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    private readonly SeqId _seqId = new(TestEpoch);

    private const int TimestampShift = 22; // 63 - 41 = 22
    private const long TimestampMask = 0x1FFFFFFFFFF; // 41 bits mask
    private const int SeqShift = 14; // TimestampShift - 8 = 14
    private const long SeqMask = 0xFF; // 8 bits mask
    private const long RandMask = 0x3FFF; // 14 bits mask (since 2^14 = 16384)

    [Fact]
    public void NextId_ShouldHaveCorrectBitStructure()
    {
        // Arrange
        var time = TestEpoch.AddMilliseconds(500);

        // Act
        long id = _seqId.Next(time);

        // Assert
        long timestampPart = (id >> TimestampShift) & TimestampMask;
        long sequencePart = (id >> SeqShift) & SeqMask;
        long randomPart = id & RandMask;

        timestampPart.ShouldBe(500);
        sequencePart.ShouldBe(0);
        randomPart.ShouldBeInRange(0, RandMask);
    }

    [Fact]
    public void NextId_ShouldIncrementSequenceForSameTimestamp()
    {
        // Arrange
        var time = TestEpoch.AddMilliseconds(100);

        // Act
        long id1 = _seqId.Next(time);
        long id2 = _seqId.Next(time);

        // Assert
        long sequence1 = (id1 >> SeqShift) & SeqMask;
        long sequence2 = (id2 >> SeqShift) & SeqMask;

        sequence2.ShouldBe(sequence1 + 1);
    }

    [Fact]
    public void NextId_ShouldResetSequenceForNewTimestamp()
    {
        // Arrange
        var time1 = TestEpoch.AddMilliseconds(100);
        var time2 = time1.AddMilliseconds(1);

        // Act
        _ = _seqId.Next(time1); // Sequence increments to 0
        _ = _seqId.Next(time1); // Sequence increments to 1
        long id = _seqId.Next(time2); // Should reset to 0

        // Assert
        long sequence = (id >> SeqShift) & SeqMask;
        sequence.ShouldBe(0);
    }

    [Fact]
    public void NextId_ShouldThrowWhenTimestampDecreases()
    {
        // Arrange
        var time1 = TestEpoch.AddMilliseconds(200);
        var time2 = TestEpoch.AddMilliseconds(100);

        // Act & Assert
        _seqId.Next(time1); // First call sets last timestamp
        Action act = () => _seqId.Next(time2);
        act.ShouldThrow<InvalidOperationException>()
            .WithMessage("Refused to create new SeqId. Last timestamp is in the future.");
    }

    [Fact]
    public void NextId_ShouldThrowWhenSequenceExhausted()
    {
        // Arrange
        var time = TestEpoch.AddMilliseconds(200);

        // Act & Assert
        for (int i = 0; i < 255; i++)
        {
            _seqId.Next(time); // Exhauste sequence
        }
        Action act = () => _seqId.Next(time);
        act.ShouldThrow<IndexOutOfRangeException>()
            .WithMessage("Refused to create new SeqId. Sequence exhausted.");
    }

    [Fact]
    public void NextId_WithStrongEntropy_ShouldSetLower14Bits()
    {
        // Arrange
        var time = TestEpoch.AddMilliseconds(300);

        // Act
        long id = _seqId.Next(time, RngEntropy.Strong);

        // Assert
        long randomPart = id & RandMask;
        randomPart.ShouldBeInRange(0, RandMask);
    }

    [Fact]
    public void NextId_WithWeakEntropy_ShouldSetLower14Bits()
    {
        // Arrange
        var time = TestEpoch.AddMilliseconds(400);

        // Act
        long id = _seqId.Next(time, RngEntropy.Weak);

        // Assert
        long randomPart = id & RandMask;
        randomPart.ShouldBeInRange(0, RandMask);
    }

    [Fact]
    public void DefaultInstance_NextId_ShouldUseDefaultEpoch()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var defaultEpoch = SeqId.DefaultEpoch;
        long expectedTimestamp = ((long)(now - defaultEpoch).TotalMilliseconds) & TimestampMask; // Mask handles overflow

        // Act
        long id = SeqId.NextId();

        // Assert
        long timestampPart = (id >> TimestampShift) & TimestampMask;
        timestampPart.ShouldBeInRange(expectedTimestamp, expectedTimestamp + 1);
    }
}