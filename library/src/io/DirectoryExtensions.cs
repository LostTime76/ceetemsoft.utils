namespace CeetemSoft.Io;

/// <summary>
/// Provides a set of <see cref="Directory"/> extension members
/// </summary>
public static class DirectoryExtensions
{
	extension(Directory)
	{
		/// <summary>
		/// Deletes a directory if it exists
		/// </summary>
		/// <param name="directory">
		/// The directory to delete
		/// </param>
		/// <param name="recursive">
		/// True to delete the target directory and everything contained within it, false to
		/// delete only the empty an empty directory
		/// </param>
		public static void DeleteIfExists(string directory, bool recursive = true)
		{
			if (!Directory.Exists(directory))
			{
				return;
			}

			Directory.Delete(directory, recursive);
		}
	}
}