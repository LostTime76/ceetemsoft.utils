namespace CeetemSoft.Utils;

/// <summary>
/// Provides an attribute for basic information about a git repository
/// </summary>
[AttributeUsage(AttributeTargets.Assembly)]
public sealed class BasicGitInfoAttribute : Attribute
{
	/// <summary>
	/// Creates a new attribute that includes basic git repository information
	/// </summary>
	/// <param name="directory">
	/// The directory containing the repository to retrieve the information of or null to use
	/// the current working directory
	/// </param>
	public BasicGitInfoAttribute(string? directory = null)
	{
		Info = BasicGitInfo.Retrieve(directory);
	}

	/// <summary>
	/// Gets the git repository information
	/// </summary>
	public BasicGitInfo? Info { get; private init; }
}