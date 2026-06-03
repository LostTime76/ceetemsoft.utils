namespace CeetemSoft.Utils;

/// <summary>
/// Provides an attribute that describes the git commit hash of an assembly
/// </summary>
[AttributeUsage(AttributeTargets.Assembly)]
public sealed class GitCommitHashAttribute : Attribute
{
	/// <summary>
	/// Creates a new attribute with the specified commit
	/// </summary>
	/// <param name="commitHash">
	/// The commit hash of the assembly. If the value is null the git process is invoked to
	/// retrieve a commit hash
	/// </param>
	/// <param name="directory">
	/// If the git process is invoked, the directory containing the repository to get the commit
	/// hash of or null to use the current working directory, otherwise ignored
	/// </param>
	public GitCommitHashAttribute(string? commitHash = null, string? directory = null)
	{
		CommitHash = commitHash ?? GitUtils.GetCommit(directory);
	}

	/// <summary>
	/// Gets the commit hash of the assembly
	/// </summary>
	public string? CommitHash { get; private init; }
}