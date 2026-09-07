using System.Diagnostics;

namespace CeetemSoft.Processes;

/// <summary>
/// Provides a set of <see cref="Process"/> extension members
/// </summary>
public static class ProcessExtensions
{
	private delegate void ArgumentsAddFunc(ProcessStartInfo settings);

	extension(Process)
	{
		/// <summary>
		/// Runs an executable using the given command and working directory
		/// </summary>
		/// <param name="command">
		/// An enumerable containing the command line invocation for the executable. The first
		/// element within the enumerable is interpreted as the executable filename. Subsequent
		/// elements are arguments passed to the executable.
		/// </param>
		/// <param name="directory">
		/// The working directory of the process
		/// </param>
		/// <returns>
		/// The result of the process invocation
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="command"/>null
		/// </exception>
		/// <exception cref="ArgumentException">
		/// Thrown if the executable filename is null or contains only whitespace
		/// </exception>
		public static ProcessResult Exec(IEnumerable<string> command, string? directory = null)
		{
			ArgumentNullException.ThrowIfNull(command, nameof(command));

			var executable = command.FirstOrDefault();

			ArgumentException.ThrowIfNullOrWhiteSpace(executable, nameof(executable));

			var settings = new ProcessStartInfo()
			{
				FileName               = executable,
				WorkingDirectory       = directory ?? string.Empty,
				RedirectStandardInput  = true,
				RedirectStandardOutput = true,
				RedirectStandardError  = true
			};

			foreach(var argument in command.Skip(1))
			{
				settings.ArgumentList.Add(argument);
			}

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