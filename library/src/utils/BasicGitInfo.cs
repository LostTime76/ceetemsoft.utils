using LibGit2Sharp;

namespace CeetemSoft.Utils;

/// <summary>
/// Provides basic information about a git repository
/// </summary>
public readonly struct BasicGitInfo
{
	/// <summary>
	/// Retrieves basic information about a git repository
	/// </summary>
	/// <param name="directory">
	/// A directory within the tree of the git repository to get the information of or null to use
	/// the current working directory
	/// </param>
	/// <returns></returns>
	public static BasicGitInfo? Retrieve(string? directory = null)
	{
		try
		{
			// Find the directory containing the repository
			directory = Repository.Discover(directory ?? Directory.GetCurrentDirectory());

			// Create the repository
			var repository = new Repository(directory);
			var status     = repository.RetrieveStatus();
			var head       = repository.Head;

			return new()
			{
				IsDirty = status.IsDirty,
				AheadBy = head.TrackingDetails.AheadBy ?? 0,
				Branch  = head.FriendlyName,
				Commit  = head.Tip.Sha
			};
		}
		catch
		{
			return null;
		}
	}

	/// <summary>
	/// Gets a value that indicates if the repository has local uncomitted changes
	/// </summary>
	public bool IsDirty { get; private init; }

	/// <summary>
	/// Gets a value that indicates the number of commits that have not yet been pushed to the
	/// repository
	/// </summary>
	public int AheadBy { get; private init; }

	/// <summary>
	/// Gets the name of the current branch within the repository
	/// </summary>
	public string? Branch { get; private init; }

	/// <summary>
	/// Gets the hash of the current commit within the repository
	/// </summary>
	public string? Commit { get; private init; }
}