using CeetemSoft.Utils;

namespace Test;

/// <summary>
/// Provides the paths for the project
/// </summary>
public static class  Paths
{
	/// <summary>
    /// Gets the name of the version source file
    /// </summary>
	public const string VersionFilename = "version.c";

	/// <summary>
	/// Gets the C project directory
	/// </summary>
	public static readonly string ProjectDirectory = Path.Combine(
			PathUtils.GetDirectoryNameOrEmpty(PathUtils.GetCurrentSourceDirectory()), "cproj");

	/// <summary>
    /// Gets the path to the source directory
    /// </summary>
	public static readonly string SourceDirectory = Path.Combine(ProjectDirectory, "src");

	/// <summary>
    /// Gets the path to the build output directory
    /// </summary>
	public static readonly string OutputDirectory = Path.Combine(ProjectDirectory, "artifacts");

	/// <summary>
    /// Gets the path to the objects output directory
    /// </summary>
	public static readonly string ObjectsDirectory = Path.Combine(OutputDirectory, "objects");

	/// <summary>
	/// Gets the path to the version source code file
	/// </summary>
	public static readonly string VersionSource = Path.Combine(SourceDirectory, VersionFilename);

	/// <summary>
    /// Gets the path to the header dependencies database
    /// </summary>
	public static readonly string DependsFilepath = Path.Combine(OutputDirectory, "hdb.json");

	/// <summary>
    /// Gets the path to the linked output file
    /// </summary>
	public static readonly string OutputFilepath = Path.Combine(OutputDirectory, "test.out");

	/// <summary>
    /// Gets the path to the linker configuration script
    /// </summary>
	public static readonly string LinkerScriptFilepath = Path.Combine(ProjectDirectory, "link.ld");

	/// <summary>
    /// Gets the path to the binary output file
    /// </summary>
	public static readonly string BinaryFilepath = Path.Combine(OutputDirectory, "test.bin");
}