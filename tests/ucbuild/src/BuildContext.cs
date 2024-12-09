using CeetemSoft.UcBuild;
using Microsoft.Extensions.FileSystemGlobbing;

namespace Test;

/// <summary>
/// Provides the context for the build
/// </summary>
public static class BuildContext
{
	private const string SourcePattern = "/**/*.c";

	/// <summary>
    /// Gets the list of source files for the build
    /// </summary>
	public static readonly List<CompileTarget> Sources = GetSources();

	/// <summary>
	/// Gets the version target for the build
	/// </summary>
	public static readonly VersionTarget VersionTarget = new(Paths.VersionSource);

	/// <summary>
    /// Gets the link target for the build
    /// </summary>
	public static readonly LinkTarget LinkTarget = new();

	/// <summary>
    /// Gets the binary generation target for the build
    /// </summary>
	public static readonly BinaryTarget BinaryTarget = new();

	/// <summary>
	/// Gets the header dependencies database
	/// </summary>
	public static readonly DependsDatabase Depends = new(Paths.DependsFilepath);

	/// <summary>
    /// Saves the build context
    /// </summary>
	public static void Save()
	{
		Depends.Save(Paths.DependsFilepath);
	}

	private static List<CompileTarget> GetSources()
	{
		// Create a matcher
		var     sources = new List<CompileTarget>();
		Matcher matcher = new();

		// Add the source pattern to the matcher
		matcher.AddInclude(SourcePattern);

		// Exclude version source
		matcher.AddExclude(Paths.VersionFilename);

		// Create the compile targets
		foreach (string source in matcher.GetResultsInFullPath(Paths.SourceDirectory))
		{
			sources.Add(new CompileTarget(source));
		}

		// Return the sources
		return sources;
	}

	/// <summary>
    /// Gets or sets the number of source files that have been built
    /// </summary>
	public static int SourcesBuilt { get; set; }

	/// <summary>
    /// Gets or sets the number of source files that need to be built
    /// </summary>
	public static int SourcesToBuild { get; set; }
}