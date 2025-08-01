namespace Just.Core;

/// <summary>
/// Specifies the quality of random entropy used in ID generation
/// </summary>
public enum RngEntropy
{
    /// <summary>
    /// Cryptographically secure random numbers (slower but collision-resistant)
    /// </summary>
    Strong,
    /// <summary>
    /// Standard pseudo-random numbers (faster but less collision-resistant)
    /// </summary>
    Weak
}
