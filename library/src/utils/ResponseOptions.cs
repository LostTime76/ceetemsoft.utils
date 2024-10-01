using System.Text;

namespace CeetemSoft.Utils;

/// <summary>
/// Provides a means to configure and create a response file that can be used to provide command
/// line arguments to a process through a file
/// </summary>
public readonly struct ResponseOptions
{
	/// <summary>
    /// Creates a new instance of the options
    /// </summary>
	public ResponseOptions() { }

	/// <summary>
    /// Creates a new response file that contains the given arguments
    /// </summary>
    /// <param name="destination">
    /// The destination path of the response file. If <see cref="FileExt"/> is non null, it is
    /// appended to the destination path. 
    /// </param>
    /// <param name="arguments">
    /// The arguments to write to the response file.
    /// </param>
    /// <returns>
    /// The command line arguments that can be used to invoke a process using the response file
    /// </returns>
	public string CreateFile(string destination, string? arguments)
	{
		// Get the path to the file
		string filepath = destination + FileExt ?? string.Empty;

		// Format the arguments
		arguments = FormatArguments(arguments);

		// Create the file
		FileUtils.WriteAllTextIfDifferent(filepath, arguments);

		// Create the file specifier
		filepath = Sclex.Escape((FileSpecifier ?? string.Empty) + filepath);

		// Return the command line arguments to use the file for process invocation
		return Argument != null ? $"{Argument} {filepath}" : filepath;
	}

	private string FormatArguments(string? arguments)
	{
		if (string.IsNullOrEmpty(arguments))
		{
			return string.Empty;
		}

		// Escape the arguments
		string[] array = EscapeArguments(arguments);

		// If the arguments are not lined, we can recombine them
		if (!LinedArguments || (array.Length == 0))
		{
			return Sclex.Join(array);
		}

		StringBuilder text = new();

		// Write the first argument
		text.Append(Sclex.Escape(array[0]));

		// Put all of the arguments on their own line
		for (int idx = 1; idx < array.Length; idx++)
		{
			text.Append(Environment.NewLine);
			text.Append(Sclex.Escape(array[idx]));
		}

		return text.ToString();
	}

	private string[] EscapeArguments(string arguments)
	{
		// Split the arguments into an array
		string[] array = Sclex.Split(arguments).ToArray();

		// Check if backslashes need to be escaped
		if (EscapeBackslashes)
		{
			for (int idx = 0; idx < array.Length; idx++)
			{
				array[idx] = array[idx].Replace("\\", "\\\\");
			}
		}

		return array;
	}

	/// <summary>
    /// Gets or initializes a value that specifies if each argument within the file should be on
    /// its own line. The default value of this property is true.
    /// </summary>
	public bool LinedArguments { get; init; } = true;

	/// <summary>
    /// Gets or initializes a value that specifies if backslashes should be escaped within each
    /// argument. The default value of this property is true.
    /// </summary>
	public bool EscapeBackslashes { get; init; } = true;

	/// <summary>
    /// Gets or initializes the command line argument used to invoke a process using the response
    /// file
    /// </summary>
	public string? Argument { get; init; }

	/// <summary>
    /// Gets or initializes the file specifier prepended to the filepath when producing the command
    /// line arguments used to invoke a process using the response file
    /// </summary>
	public string? FileSpecifier { get; init; }

	/// <summary>
    /// Gets or initializes the file extension to be appended to the destination path when creating
    /// the response file
    /// </summary>
	public string? FileExt { get; init; }
}