using System.Diagnostics;

namespace CeetemSoft.Processes;

/// <summary>
/// Describes the reasult of a process invocation
/// </summary>
public readonly struct ProcessResult
{
	/// <summary>
	/// Gets the exit code of the process
	/// </summary>
	public int ExitCode { get; init; }

	/// <summary>
	/// Gets the output of the process
	/// </summary>
	public string? Output { get; init; }

	/// <summary>
	/// Gets the error output of the process
	/// </summary>
	public string? Error { get; init; }

	/// <summary>
	/// Gets the settings that the process was invoked with
	/// </summary>
	public ProcessStartInfo? Settings { get; init; }
}