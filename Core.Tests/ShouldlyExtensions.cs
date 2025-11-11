namespace Just.Core.Tests;

public static class ShouldlyExtensions
{
    public static TException WithMessage<TException>(this TException exception, string expectedMessage, string? customMessage = null)
        where TException : Exception
    {
        exception.Message.ShouldBe(expectedMessage, customMessage: customMessage);
        return exception;
    }
}
