using CeetemSoft.Processes;
using System.Diagnostics;

namespace CeetemSoft.Utils;

/// <summary>
/// Provides a set of git utility members
/// </summary>
public static class GitUtils
{
	private const string Executable       = "git";
	private const string GetCommitCommand = "rev-parse HEAD";

	/// <summary>
	/// Attempts to get the commit string of the current branch within a repository
	/// </summary>
	/// <param name="directory">
	/// A directory within the repository tree to run the git command within. If no directory is
	/// given, the current working directory is used.
	/// </param>
	/// <returns>
	/// The 40 character commit string of the branch
	/// </returns>
	public static string GetCommit(string? directory = null)
	{
		var result = Process.Exec(Executable, GetCommitCommand);

		return (result.Output ?? string.Empty).Trim();
	}
}