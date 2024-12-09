using CeetemSoft.UcBuild;
using CeetemSoft.Utils;

namespace Test;

/// <summary>
/// Implements a build target that compiles a source file
/// </summary>
public class CompileTarget : BuildTarget
{
	private const string ObjectFileExt  = ".o";
	private const string DependsFileExt = ".d";

	private static readonly string BaseRule = Sclex.Join([
		"clang", "-c", "--target=armv7m-none-eabi",
		"-o", "{1}", "-MMD", "-MF", "{2}", "{0}"
	]);

	/// <summary>
	/// Creates a new target
	/// </summary>
	/// <param name="source">
	/// The path to the source file to compile
	/// </param>
	public CompileTarget(string source)
	{
		Source = source;
	}

	protected override void Executed()
	{
		string source = Path.GetFileName(Source);
		string obj    = Path.GetFileName(Object!);
		string output = Result.Output + Result.Error;

		// Get job info
		int job   = ++BuildContext.SourcesBuilt;
		int total = BuildContext.SourcesToBuild;

		// Print a short command line
		Console.WriteLine($"[{job}/{total}] clang \"{source}\" -> \"{obj}\"");

		// Print any additional output
		ConsoleUtils.WriteLineIfNotEmpty(output);
	}

	protected override bool Prepare()
	{
		// Use the source filename to create a unique destination
		string filename = Path.GetFileNameWithoutExtension(Source);
		string relative = Path.GetRelativePath(Paths.SourceDirectory, Source);

		// Create the unique output destination
		string destination = Path.Combine(
			Paths.ObjectsDirectory,	PathUtils.GetDirectoryNameOrEmpty(relative), filename);

		// Set the paths to the output files
		Object  = destination + ObjectFileExt;
		Depends = destination + DependsFileExt;

		// Get the source file timestamp
		long timestamp = FileUtils.GetTimestamp(Source);

		// Determine if the object file is out of data
		if (FileUtils.IsOlder(Object, timestamp))
		{
			return true;
		}

		// Determine if the header dependencies file is outdated
		else if (FileUtils.IsOlder(Depends, timestamp))
		{
			return true;
		}

		// Determine if the header dependencies are outdated
		return BuildContext.Depends.AreDependsOutdated(Depends, timestamp);
	}

	protected override bool Execute()
	{
		// Ensure the output directory exists
		DirectoryUtils.TryCreateDirectory(Path.GetDirectoryName(Object));

		// Generate the command line
		string objPath  = Sclex.Escape(Object);
		string source   = Sclex.Escape(Source);
		string depsPath = Sclex.Escape(Depends);
		string command  = string.Format(BaseRule, source, objPath, depsPath);

		// Invoke the compiler
		if ((Result = ProcessUtils.Execute(command)).ExitCode == 0)
		{
			// Update header dependencies
			BuildContext.Depends.UpdateDepends(depsPath);
			return true;
		}

		return false;
	}	

	/// <summary>
    /// Gets the path to the source file
    /// </summary>
	public string Source { get; private init; }

	/// <summary>
    /// Gets the path to the output object file
    /// </summary>
	public string? Object { get; private set; }

	/// <summary>
    /// Gets the path to the output dependencies header file
    /// </summary>
	public string? Depends { get; private set; }

	/// <summary>
    /// Gets the result of the compiler invocation
    /// </summary>
	public ProcessResult Result { get; private set; }
}