using System.Runtime.CompilerServices;

namespace CeetemSoft.Utils;

/// <summary>
/// Provides a set of path utility functions
/// </summary>
public static class PathUtils
{
	/// <summary>
    /// Gets the path of the current source file which makes the call
    /// </summary>
    /// <param name="source">
    /// This parameter should always be null as it is filled in by the compiler at compile time
    /// </param>
    /// <returns>
    /// The path of source file which made the call
    /// </returns>
	public static string GetCurrentSourcePath([CallerFilePath] string? source = null)
	{
		return source!;
	}

	/// <summary>
    /// Gets the directory of the current source file which makes the call
    /// </summary>
    /// <param name="source">
    /// This parameter should always be null as it is filled in by the compiler at compile time
    /// </param>
    /// <returns>
    /// The directory of the source file which made the call
    /// </returns>
	public static string GetCurrentSourceDirectory([CallerFilePath] string? source = null)
	{
		return GetDirectoryNameOrEmpty(source);
	}

	/// <summary>
    /// Gets the name of a directory from a path or an empty string
    /// </summary>
    /// <param name="path">
    /// The path to get the directory name of
    /// </param>
    /// <returns>
    /// The directory name of the path if successful, otherwise empty
    /// </returns>
	public static string GetDirectoryNameOrEmpty(string? path)
	{
		return Path.GetDirectoryName(path) ?? string.Empty;
	}
}