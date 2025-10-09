namespace CeetemSoft.Processes;

/// <summary>
/// Describes the result of a process invocation
/// </summary>
public readonly struct ProcessResult
{
	/// <summary>
	/// Gets a value that indicates if the process exited successfully,
	/// that is if <see cref="ExitCode"/> is 0
	/// </summary>
	public bool Success => ExitCode == 0;

	/// <summary>
	/// Gets or initializes the exit code of the process
	/// </summary>
	public required int ExitCode { get; init; }

	/// <summary>
	/// Gets or initializes the process output text
	/// </summary>
	public string? Output { get; init; }

	/// <summary>
	/// Gets or initializes the process error text
	/// </summary>
	public string? Error { get; init; }
}