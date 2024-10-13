namespace CeetemSoft.Utils;

/// <summary>
/// Provides a means to describe and generate response files that can be used to invoke a process
/// </summary>
public readonly struct ResponseOptions
{
	/// <summary>
    /// Gets the default file prefix
    /// </summary>
	public const string DefaultFilePrefix = "@";

	/// <summary>
    /// Gets the default response file extension
    /// </summary>
	public const string DefaultFileExt = ".rsp";

	/// <summary>
	/// Gets the default response file options
	/// </summary>
	public static readonly ResponseOptions Default = new()
	{
		EscapeBackslashes = true,
		SplitArguments    = true,
		FileExt           = DefaultFileExt,
		FilePrefix        = DefaultFilePrefix
	};

	/// <summary>
    /// Creates new options
    /// </summary>
	public ResponseOptions() { }

	/// <summary>
    /// Creates a response file containing the command line arguments from a command
    /// </summary>
    /// <param name="filepath">
    /// The path of the response file to create
    /// </param>
    /// <param name="command">
    /// The process command to execute
    /// </param>
    /// <returns>
    /// The process command that can be used to invoke a process using the generated response file
    /// </returns>
	public string CreateFileForCommand(string filepath, string command)
	{
		ArgumentException.ThrowIfNullOrEmpty(command, nameof(command));

		// Split the command
		(string executable, string arguments) = Sclex.SplitCommand(command);

		// Create the file
		command = CreateFileWithArguments(filepath, arguments);

		// Resolve the command
		return string.Join(Sclex.DefaultSeparator, Sclex.Escape(executable), command);
	}

	/// <summary>
    /// Creates a response file containing the specified arguments
    /// </summary>
    /// <param name="filepath">
    /// The path of the response file to create
    /// </param>
    /// <param name="arguments">
    /// The arguments to write to the response file
    /// </param>
    /// <returns>
    /// The command line arguments that can be used to invoke a process using the generated
    /// response file
    /// </returns>
	public string CreateFileWithArguments(string filepath, string? arguments = null)
	{
		// Write the file
		FileUtils.WriteAllTextIfDifferent(filepath, ResolveArguments(arguments));

		// Create the arguments
		return Sclex.Join(Argument, (FilePrefix ?? string.Empty) + filepath);
	}

	private string ResolveArguments(string? arguments)
	{
		if (string.IsNullOrEmpty(arguments) || (!EscapeBackslashes && !SplitArguments))
		{
			return arguments ?? string.Empty;
		}

		// Split the arguments
		List<string> list = Sclex.Split(arguments);

		// Escape backslashes
		if (EscapeBackslashes)
		{
			EscapeArgumentBackslashes(list);
		}

		// Rejoin the arguments
		return Sclex.Join(list, SplitArguments ? Environment.NewLine : Sclex.DefaultSeparator);
	}

	private static void EscapeArgumentBackslashes(List<string> arguments)
	{
		for (int idx = 0; idx < arguments.Count; idx++)
		{
			arguments[idx] = arguments[idx].Replace("\\", "\\\\");
		}
	}

	/// <summary>
    /// Gets or initializes a boolean flag that indicates if backslahes within the arguments should
    /// be escaped when written to the response file. For example: 'a\foo.c' becomes 'a\\foo.c'.
    /// This property is set to true for the default options.
    /// </summary>
	public bool EscapeBackslashes { get; init; }

	/// <summary>
    /// Gets or initializes a boolean flag that indicates if the arguments written to the response
    /// file should be split onto their own lines. This property is set to true for the default
    /// options.
    /// </summary>
	public bool SplitArguments { get; init; }

	/// <summary>
    /// Gets or initializes the file extension for created response files. This property is set to
    /// <see cref="DefaultFileExt"/> for the default options.
    /// </summary>
	public string? FileExt { get; init; }

	/// <summary>
	/// Gets or initializes the argument used to invoke a process using the command line. If this
	/// property is null, no argument is used.
	/// </summary>
	public string? Argument { get; init; }

	/// <summary>
    /// Gets or initializes a prefix that is added to the name of the response file when passing
    /// it as an argument to a process. This property is set to <see cref="DefaultFilePrefix"/> for
    /// the default options.
    /// </summary>
	public string? FilePrefix { get; init; }
}