using System.Diagnostics;

namespace CeetemSoft.Processes;

/// <summary>
/// Provides a set of process extension members
/// </summary>
public static class ProcessExtensions
{
	/// <summary>
    /// Gets the maximum number of characters that can be passed within a command line
    /// </summary>
	public const int MaxCommandLineLength = 32000;

	extension(Process)
	{
		/// <summary>
		/// Runs a process executable using the current working directory
		/// </summary>
		/// <param name="executable">
		/// The process executable to run
		/// </param>
		/// <returns>
		/// The result of the process invocation
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="executable"/> is null or empty
		/// </exception>
		public static ProcessResult Exec(string executable) => Exec(executable, null, null);

		/// <summary>
		/// Runs a process executable with command line arguments using the current working
		/// directory
		/// </summary>
		/// <param name="executable">
		/// The process executable to run
		/// </param>
		/// <param name="arguments">
		/// The command line arguments to pass to the process
		/// </param>
		/// <returns>
		/// The result of the process invocation
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="executable"/> is null or empty
		/// </exception>
		public static ProcessResult Exec(string executable, string? arguments) =>
			Exec(executable, null, arguments);

		/// <summary>
		/// Runs a process executable with command line arguments and a specified working directory
		/// </summary>
		/// <param name="executable">
		/// The process executable to run
		/// </param>
		/// <param name="directory">
		/// The working directory of the process. If the value is null or empty, the current
		/// working directory is used.
		/// </param>
		/// <param name="arguments">
		/// The command line arguments to pass to the process
		/// </param>
		/// <returns>
		/// The result of process invocation
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="executable"/> is null or empty
		/// </exception>
		public static ProcessResult Exec(string executable, string? directory, string? arguments)
		{
			ArgumentNullException.ThrowIfNull(executable, nameof(executable));

			Process process = new() { StartInfo = new() {
				FileName               = executable,
				WorkingDirectory       = GetWorkingDirectory(directory),
				Arguments              = arguments ?? string.Empty,
				RedirectStandardInput  = true,
				RedirectStandardOutput = true,
				RedirectStandardError  = true
			}};

			process.Start();

			string? output = null;
			string? error  = null;

			Thread othread = Thread.Start(() => output = process.StandardOutput.ReadToEnd());
			Thread ethread = Thread.Start(() => error = process.StandardError.ReadToEnd());

			process.StandardInput.Close();
			process.WaitForExit();
			othread.Join();
			ethread.Join();

			return new() {
				ExitCode = process.ExitCode,
				Output   = string.IsNullOrEmpty(output) ? null : output,
				Error    = string.IsNullOrEmpty(error) ? null : error
			};
		}
	}

	private static string GetWorkingDirectory(string? directory) =>
		string.IsNullOrEmpty(directory) ? Directory.GetCurrentDirectory() : directory;
}