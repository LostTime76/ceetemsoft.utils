namespace CeetemSoft.Io;

/// <summary>
/// Provides a set of directory extension members
/// </summary>
public static class DirectoryExtensions
{
	/// <summary>
	/// Provides a set of directory extension members
	/// </summary>
	extension(Directory)
	{
		/// <summary>
		/// Creates a directory if it does not already exist
		/// </summary>
		/// <param name="directory">
		/// The path to the directory to create
		/// </param>
		/// <returns>
		/// True if the directory was created, false otherwise
		/// </returns>
		public static bool CreateIfMissing(string? directory)
		{
			if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
			{
				Directory.CreateDirectory(directory);
				return true;
			}

			return false;
		}

		/// <summary>
		/// Deletes a directory if it exists
		/// </summary>
		/// <param name="directory">
		/// The path to the directory to delete
		/// </param>
		/// <returns>
		/// True if the directory was deleted, false otherwise
		/// </returns>
		public static bool DeleteIfExists(string? directory)
		{
			if (Directory.Exists(directory))
			{
				Directory.Delete(directory, true);
				return true;
			}

			return false;
		}
	}
}