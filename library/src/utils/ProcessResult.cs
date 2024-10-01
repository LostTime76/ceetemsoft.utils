namespace CeetemSoft.Utils;

/// <summary>
/// Provides a structure to obtain the result of invoking a process
/// </summary>
public readonly struct ProcessResult
{
	/// <summary>
    /// Gets or initializes the exit code of the process
    /// </summary>
	public int ExitCode { get; init; }

	/// <summary>
    /// Gets or initializes the standard output string retrieved from the process
    /// </summary>
	public string? Output { get; init; }

	/// <summary>
    /// Gets or initializes the standard error string retrieved from the process
    /// </summary>
	public string? Error { get; init; }
}