namespace CeetemSoft.Utils;

/// <summary>
/// Provides an attribute that can store program build information
/// </summary>
[AttributeUsage(AttributeTargets.Assembly)]
public class BuildInfoAttribute : Attribute
{
	/// <summary>
    /// Gets or sets the version string
    /// </summary>
	public string? Version { get; set; }

	/// <summary>
    /// Gets or sets the build time string
    /// </summary>
	public string? BuildTime { get; set; }

	/// <summary>
	/// Gets or sets the author string
	/// </summary>
	public string? Author { get; set; }

	/// <summary>
	/// Gets or sets the commit string
	/// </summary>
	public string? Commit { get; set; }
}