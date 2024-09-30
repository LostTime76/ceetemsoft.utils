using System.Diagnostics.CodeAnalysis;

namespace CeetemSoft.Utils;

/// <summary>
/// Provides a set of directory utility functions
/// </summary>
public static class DirectoryUtils
{
	/// <summary>
    /// Tries to create a directory if it does not already exist
    /// </summary>
    /// <param name="directory">
    /// The path to the directory to create
    /// </param>
    /// <returns>
    /// True if the directory exists or is created successfully, false otherwise
    /// </returns>
	public static bool TryCreateDirectory([NotNullWhen(true)]string? directory)
	{
		if (string.IsNullOrEmpty(directory))
		{
			return false;
		}

		try	{ Directory.CreateDirectory(directory); }
		catch {	return false; }

		return true;
	}
}