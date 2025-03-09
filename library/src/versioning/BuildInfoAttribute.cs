namespace CeetemSoft.Versioning;

/// <summary>
/// Provides an attribute that can be used to track build information for a project. A set of
/// properties are provided within the attribute that can be used however the application desires.
/// </summary>
[AttributeUsage(AttributeTargets.Assembly)]
public class BuildInfoAttribute : Attribute
{
	/// <summary>
    /// Gets or initializes the version string
    /// </summary>
	public string? Version { get; init; }

	/// <summary>
    /// Gets or initializes the build time string
    /// </summary>
	public string? BuildTime { get; init; }

	/// <summary>
    /// Gets or initializes the author string
    /// </summary>
	public string? Author { get; init; }

	/// <summary>
    /// Gets or initializes the commit string
    /// </summary>
	public string? Commit { get; init; }
}