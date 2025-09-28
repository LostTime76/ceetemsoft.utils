namespace CeetemSoft.Attributes;

/// <summary>
/// Provides an attribute that stores a git commit hash
/// </summary>
[AttributeUsage(AttributeTargets.All)]
public class GitCommitHashAttribute : Attribute
{
	/// <summary>
	/// Creates a new commit hash attribute
	/// </summary>
	public GitCommitHashAttribute() { }

	/// <summary>
	/// Creates a new commit hash attribute that stores the given commit hash
	/// </summary>
	/// <param name="commitHash">
	/// The commit hash to store
	/// </param>
	public GitCommitHashAttribute(string? commitHash)
	{
		CommitHash = commitHash;
	}

	/// <summary>
	/// Gets or initializes the commit hash
	/// </summary>
	public string? CommitHash { get; init; }
}