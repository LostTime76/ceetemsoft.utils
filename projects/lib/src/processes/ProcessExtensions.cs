using System.Diagnostics;

namespace CeetemSoft.Processes;

/// <summary>
/// Provides a set of process extension members
/// </summary>
public static class ProcessExtensions
{
	/// <summary>
    /// The maximum length in characters of a command line command
    /// </summary>
	public const int MaxCommandLength = 32000;

	/// <summary>
    /// Provides a simple means to execute a command line process and retrieve its results
    /// </summary>
    /// <param name="command">
    /// The command line to execute. The first element of the command line is interpreted as the
    /// executable and the following elements are interpreted as the arguments to pass to the
    /// executable.
    /// </param>
    /// <param name="workingDirectory">
    /// The working directory for the process. If the value is null or empty, the current working
    /// directory is used.
    /// </param>
    /// <returns>
    /// The result of the process execution
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown if <paramref name="command"/> is null or empty
    /// </exception>
    /// <remarks>
    /// This function creates two threads to read both the standard output and error streams from
    /// the process as it executes. The function does not return until the process has finished and
    /// exited.
    /// </remarks>
	public static ProcessResult Run(string command, string? workingDirectory = null)
	{
		// Split the command
		(string executable, string arguments) = Sclex.SplitCommand(command);

		// Run the process
		return Run(executable, arguments, workingDirectory);
	}

	/// <summary>
    /// Provides a simple means to execute a command line process and retrieve its results
    /// </summary>
    /// <param name="executable">
    /// The path to the executable to invoke
    /// </param>
    /// <param name="arguments">
    /// The arguments to pass to the executable
    /// </param>
    /// <param name="workingDirectory">
    /// The working directory for the process. If the value is null or empty, the current working
    /// directory is used.
    /// </param>
    /// <returns>
    /// The result of the process execution
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown if <paramref name="executable"/> is null or empty
    /// </exception>
    /// <remarks>
    /// This function creates two threads to read both the standard output and error streams from
    /// the process as it executes. The function does not return until the process has finished and
    /// exited.
    /// </remarks>
	public static ProcessResult Run(
		string executable,
		string? arguments,
		string? workingDirectory = null)
	{
		ArgumentException.ThrowIfNullOrEmpty(executable, nameof(executable));

		// Create the process
		Process process = new() { StartInfo = new()
		{
			FileName               = executable,
			WorkingDirectory       = workingDirectory ?? string.Empty,
			Arguments              = arguments ?? string.Empty,
			RedirectStandardOutput = true,
			RedirectStandardError  = true
		}};

		// Start the process
		process.Start();

		// Store the process standard output and error stream data
		string? output = null;
		string? error  = null;

		// Create the standard output read thread
		Thread outputThread = new(delegate() {
			output = process.StandardOutput.ReadToEnd();
		});

		// Create the standard error read thread
		Thread errorThread = new(delegate() {
			error = process.StandardError.ReadToEnd();
		});

		// Start the threads
		outputThread.Start();
		errorThread.Start();

		// Wait for everything to complete
		process.WaitForExit();
		outputThread.Join();
		errorThread.Join();

		// Return the result of the operation
		return new()
		{
			ExitCode = process.ExitCode,
			Output   = string.IsNullOrEmpty(output) ? null : output,
			Error    = string.IsNullOrEmpty(error) ? null : error
		};
	}
}