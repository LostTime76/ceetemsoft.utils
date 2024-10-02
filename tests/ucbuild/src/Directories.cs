using CeetemSoft.Utils;

namespace Test;

/// <summary>
/// Provides the set of directories for the C project
/// </summary>
public static class Directories
{
	/// <summary>
    /// Gets the path to the C project directory
    /// </summary>
	public static readonly string Project =
		Path.GetFullPath(Path.Combine(PathUtils.GetCurrentSourceDirectory(), "..\\cproj"));

	/// <summary>
    /// Gets the path to the artifacts directory
    /// </summary>
	public static readonly string Artifacts = Path.Combine(Project, "artifacts");

	/// <summary>
    /// Gets the path to the output objects directory
    /// </summary>
	public static readonly string Objects = Path.Combine(Artifacts, "objects");

	/// <summary>
    /// Gets the path to the source directory
    /// </summary>
	public static readonly string Source = Path.Combine(Project, "src");
}