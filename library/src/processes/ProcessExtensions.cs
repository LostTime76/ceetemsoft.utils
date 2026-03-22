using System.Diagnostics;
using CeetemSoft.Utils;

namespace CeetemSoft.Processes;

/// <summary>
/// Provides a set of <see cref="Process"/> extension members
/// </summary>
public static class ProcessExtensions
{
	extension(Process)
	{
		/// <summary>
		/// Run a process executable with command line arguments and a specified working directory
		/// </summary>
		/// <param name="executable">
		/// The process executable to run
		/// </param>
		/// <param name="arguments">
		/// The command line arguments to pass to the process
		/// </param>
		/// <param name="directory">
		/// The working directory of the process. If the value is null or empty, the current
		/// working directory is used.
		/// </param>
		/// <returns>
		/// The result of the process invocation
		/// </returns>
		/// <exception cref="ArgumentException">
		/// Thrown if <paramref name="executable"/> is null, empty, or only contains whitespace
		/// characters
		/// </exception>
		public static ProcessResult Exec(
			string executable, string? arguments = null, string? directory = null)
		{
			ArgumentException.ThrowIfNullOrWhiteSpace(executable, nameof(executable));

			var settings = new ProcessStartInfo()
			{
				FileName               = executable,
				WorkingDirectory       = directory ?? string.Empty,
				Arguments              = arguments ?? string.Empty,
				RedirectStandardInput  = true,
				RedirectStandardOutput = true,
				RedirectStandardError  = true
			};

			var     process = new Process() { StartInfo = settings };
			string? output  = null;
			string? error   = null;

			process.Start();

			var othread = Thread.Start(() => output = process.StandardOutput.ReadToEnd());
			var ethread = Thread.Start(() => error = process.StandardError.ReadToEnd());

			process.StandardInput.Close();
			process.WaitForExit();
			othread.Join();
			ethread.Join();

			return new ProcessResult()
			{
				ExitCode = process.ExitCode,
				Output   = string.IsNullOrEmpty(output) ? null : output,
				Error    = string.IsNullOrEmpty(error) ? null : error,
				Settings = settings
			};
		}
	}
}