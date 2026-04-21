using CeetemSoft.Processes;
using System.Diagnostics;

namespace CeetemSoft.Utils;

/// <summary>
/// Provides a set of git utility members
/// </summary>
public static class GitUtils
{
	/// <summary>
	/// The size in bytes of a commit string
	/// </summary>
	public const int CommitSize = 40;

	/// <summary>
	/// Represents an invalid commit as a series of '0' hex characters
	/// </summary>
	public static readonly string NullCommit = new('0', CommitSize);

	private const string Executable       = "git";
	private const string GetCommitCommand = "rev-parse HEAD";

	/// <summary>
	/// Gets the commit string of the current branch within a repository
	/// </summary>
	/// <param name="directory">
	/// A directory within the repository tree to run the git command within. If no directory is
	/// given, the current working directory is used.
	/// </param>
	/// <returns>
	/// The commit string if it was retrieved successfully, otherwise null
	/// </returns>
	public static string? GetCommit(string? directory = null)
	{
		var result = Process.Exec(Executable, GetCommitCommand, directory);
		var commit = (result.Output ?? string.Empty).Trim();

		if ((result.ExitCode != 0) || (commit.Length != CommitSize))
		{
			return null;
		}

		return commit;
	}
}