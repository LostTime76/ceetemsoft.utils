using System.Diagnostics;
using System.Runtime.InteropServices;

namespace CeetemSoft.Utils;

/// <summary>
/// Provides a set of process utility functions
/// </summary>
public static class ProcessUtils
{
	/// <summary>
    /// The maximum length in characters of command line arguments passed to a process
    /// </summary>
	public const int MaxArgumentsLength = 32000;

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
	public static ProcessResult Execute(string command, string? workingDirectory = null)
	{
		// Split the command
		(string executable, string arguments) = Sclex.SplitCommand(command);

		// Ensure at least we found an executable
		ArgumentException.ThrowIfNullOrEmpty(executable, nameof(command));

		// Use the current directory if the working directory is not specified
		workingDirectory = string.IsNullOrEmpty(workingDirectory) ?
			Directory.GetCurrentDirectory() : workingDirectory;

		// Create the process
		Process process = new() { StartInfo = new()
		{
			FileName               = executable,
			WorkingDirectory       = workingDirectory,
			Arguments              = arguments,
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