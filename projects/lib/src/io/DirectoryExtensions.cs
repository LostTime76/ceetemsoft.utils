namespace CeetemSoft.Io;

/// <summary>
/// Provides a set of directory extension members
/// </summary>
public static class DirectoryExtensions
{
	/// <summary>
    /// Returns the current directory if the input directory is null or empty, otherwise the input
    /// directory is returned
    /// </summary>
    /// <param name="directory">
    /// The input directory
    /// </param>
    /// <returns>
    /// <see cref="Directory.GetCurrentDirectory()"/> if <paramref name="directory"/> is null or
    /// empty, otherwise <paramref name="directory"/>.
    /// </returns>
	public static string GetDirectoryOrCurrent(string? directory)
	{
		return string.IsNullOrEmpty(directory) ? Directory.GetCurrentDirectory() : directory;
	}

	/// <summary>
    /// Create a directory if it does not already exist
    /// </summary>
    /// <param name="directory">
    /// The path to the directory to create
    /// </param>
	public static void CreateDirectoryIfMissing(string directory)
	{
		if (!Directory.Exists(directory))
		{
			Directory.CreateDirectory(directory);
		}
	}

	/// <summary>
    /// Deletes a directory if it exists
    /// </summary>
    /// <param name="directory">
    /// The path to the directory to delete
    /// </param>
    /// <param name="recursive">
    /// True to recursively delete the directory and its contents, false to only delete the
    /// directory
    /// </param>
	public static void DeleteDirectoryIfExists(string directory, bool recursive = true)
	{
		if (Directory.Exists(directory))
		{
			Directory.Delete(directory, recursive);
		}
	}
}