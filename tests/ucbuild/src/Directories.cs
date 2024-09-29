using CeetemSoft.Utils;

namespace Test;

/// <summary>
/// Provides the set of directories for the test
/// </summary>
public static class Directories
{
	/// <summary>
    /// Gets the path to the C project directory
    /// </summary>
	public static readonly string Project =
		Path.GetFullPath(Path.Combine(PathUtils.GetCurrentSourceDirectory(), "..\\cproj"));

	/// <summary>
    /// Gets the path to the artifacts directory within the project
    /// </summary>
	public static readonly string Artifacts = Path.Combine(Project, "artifacts");

	/// <summary>
    /// Gets the path to the objects output directory within the project
    /// </summary>
	public static readonly string Objects = Path.Combine(Artifacts, "objects");
}