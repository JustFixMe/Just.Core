namespace Just.Core.Extensions;

public static class SystemStringExtensions
{
    /// <summary>
    /// Indicates whether the specified string is null or an empty string ("").
    /// </summary>
    /// <param name="str">The string to test.</param>
    /// <returns>true if the value parameter is null or an empty string (""); otherwise, false.</returns>
    public static bool IsNullOrEmpty([NotNullWhen(false)] this string? str) => string.IsNullOrEmpty(str);

    /// <summary>
    /// Indicates whether a specified string is null, empty, or consists only of white-space
    /// characters.
    /// </summary>
    /// <param name="str">The string to test.</param>
    /// <returns>true if the value parameter is null or <see cref="System.String.Empty"/>, or if value consists
    /// exclusively of white-space characters.</returns>
    public static bool IsNullOrWhiteSpace([NotNullWhen(false)] this string? str) => string.IsNullOrWhiteSpace(str);


    /// <summary>
    /// Indicates whether the specified string contains any characters.
    /// </summary>
    /// <param name="str">The string to test.</param>
    /// <returns>true if the value parameter contains any characters; otherwise, false.</returns>
    public static bool IsNotNullOrEmpty([NotNullWhen(true)] this string? str) => !string.IsNullOrEmpty(str);

    /// <summary>
    /// Indicates whether a specified string contains non white-space characters.
    /// </summary>
    /// <param name="str">The string to test.</param>
    /// <returns>true if the value parameter contains non white-space characters; otherwise, false.</returns>
    public static bool IsNotNullOrWhiteSpace([NotNullWhen(true)] this string? str) => !string.IsNullOrWhiteSpace(str);
    
}
