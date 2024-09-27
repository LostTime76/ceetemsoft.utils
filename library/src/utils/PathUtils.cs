namespace CeetemSoft.Utils;

/// <summary>
/// Provides a set of path utility functions
/// </summary>
public static class PathUtils
{
	/// <summary>
    /// Gets the common directory of all the paths within a span
    /// </summary>
    /// <param name="paths">
    /// The span containing the paths to get the common directory of
    /// </param>
    /// <returns>
    /// The common directory of all the paths within <paramref name="paths"/> or empty if
    /// <paramref name="paths"/> is empty
    /// </returns>
	public static string GetCommonDirectory(ReadOnlySpan<string> paths)
	{
		return Path.GetDirectoryName(StringUtils.GetCommonPrefix(paths)) ?? string.Empty;
	}

	/// <summary>
    /// Gets the common directory of all the paths within an array
    /// </summary>
    /// <param name="paths">
    /// The array containing the paths to get the common directory of
    /// </param>
    /// <returns>
    /// The common directory of all the paths within <paramref name="paths"/> or empty if
    /// <paramref name="paths"/> is null
    /// </returns>
	public static string GetCommonDirectory(params string[]? paths)
	{
		return GetCommonDirectory(paths.AsSpan());
	}

	/// <summary>
    /// Gets the common directory of all the paths within an enumerable
    /// </summary>
    /// <param name="paths">
    /// The enumerable containing the paths to get the common directory of
    /// </param>
    /// <returns>
    /// The common directory of all the paths within <paramref name="paths"/> or empty if
    /// <paramref name="paths"/> is null
    /// </returns>
	public static string GetCommonDirectory(IEnumerable<string>? paths)
	{
		return Path.GetDirectoryName(StringUtils.GetCommonPrefix(paths)) ?? string.Empty;
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