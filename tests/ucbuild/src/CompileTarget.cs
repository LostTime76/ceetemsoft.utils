using System.IO.Enumeration;
using CeetemSoft.UcBuild;
using CeetemSoft.Utils;

namespace Test;

public class CompileTarget : BuildTarget
{
	private const int    FlagsIdx   = 1;
	private const string Executable = "clang";
	private const string ObjectExt  = ".o";
	private const string DependsExt = ".d";

	private static readonly string[] _baseArgs = [
		"-c", "-o", "{1}", "-MMD", "-MF", "{2}", "{0}"
	];

	private static BuildRule? _defaultRule;

	public CompileTarget(string source) : this(source, DefaultRule) { }

	public CompileTarget(string source, BuildRule rule)
	{
		ArgumentException.ThrowIfNullOrEmpty(source, nameof(source));
		ArgumentNullException.ThrowIfNull(rule, nameof(rule));

		Source = source;
		Rule   = rule;
	}

	public override void Executed(int jobIndex, int totalJobs)
	{
		string rsource = Path.GetRelativePath(Directories.Source, Source);
		string robject = Path.GetRelativePath(Directories.Objects, Object!);

		// Print a short command line
		Console.WriteLine($"[{jobIndex}/{totalJobs}] {Executable} {rsource} -> {robject}");

		// Print any additional output
		ConsoleUtils.WriteLineIfNotEmpty(Result.Output + Result.Error);
	}

	public override bool CheckIfOutdated()
	{
		// Use the name of the source file to produce a unique output destination
		string rsource     = Path.GetRelativePath(Directories.Source, Source);
		string relative    = PathUtils.GetDirectoryNameOrEmpty(rsource);
		string filename    = Path.GetFileNameWithoutExtension(Source);
		string destination = Path.Combine(Directories.Objects, relative, filename);

		// Get the paths to the output files
		Object  = destination + ObjectExt;
		Depends = destination + DependsExt;

		// Get the source file timestamp
		long timestamp = FileUtils.GetTimestamp(Source);

		// Check if the build rule is outdated
		if (BuildContext.TargetDatabase!.IsRuleOutdated(Object, Rule.Format))
		{
			return true;
		}

		// Check if the object file is outdated
		else if (FileUtils.IsOlder(Object, timestamp))
		{
			return true;
		}

		// Check if the dependencies are outdated
		return BuildContext.DependsDatabase!.AreDependsOutdated(Depends, timestamp);
	}

	public override bool Execute()
	{
		// Ensure the output directory exists
		DirectoryUtils.TryCreateDirectory(Path.GetDirectoryName(Object));

		// Create the compiler arguments
		string source = Sclex.Escape(Source);
		string opath  = Sclex.Escape(Object);
		string dpath  = Sclex.Escape(Depends);
		string args   = string.Format(Rule.ArgumentFormat!, source, opath, dpath);

		return (Result = ProcessUtils.Execute(Executable, args)).ExitCode != 0;
	}

	public static BuildRule CreateRule(ReadOnlySpan<string?> flags)
	{
		return new(Executable, Sclex.Join(_baseArgs.InsertRange(FlagsIdx, flags)));
	}

	public static BuildRule CreateRule(IEnumerable<string?>? flags)
	{
		return new(Executable, Sclex.Join(_baseArgs.InsertRange(FlagsIdx, flags)));
	}

	public string Source { get; private init; }

	public BuildRule Rule { get; private init; }

	public string? Object { get; private set; }

	public string? Depends { get; private set; }

	public ProcessResult Result { get; private set; }

	public static BuildRule DefaultRule =>
		_defaultRule ??= new(Executable, Sclex.Join(_baseArgs));
}