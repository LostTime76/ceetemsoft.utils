using System.Runtime.CompilerServices;

namespace CeetemSoft.Io;

/// <summary>
/// Provides a set of path exteions members
/// </summary>
public static class PathUtils
{
	/// <summary>
    /// Normalizes a path by replacing any backslashes with forward slashes
    /// </summary>
    /// <param name="path">
    /// The path to normalize
    /// </param>
    /// <returns>
    /// If <paramref name="path"/> is not null, the normalized path is returned, otherwise
    /// <see cref="string.Empty"/> is returned.
    /// </returns>
	public static string NormalizePath(string? path)
	{
		if (string.IsNullOrEmpty(path))
		{
			return string.Empty;
		}

		return path.Replace('\\', '/');
	}

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

	/// <summary>
    /// Strips an extension from a filepath
    /// </summary>
    /// <param name="filepath">
    /// The filepath to strip the extension from
    /// </param>
    /// <returns>
    /// The filepath with the extension stripped
    /// </returns>
	public static string StripExtension(string? filepath)
	{
		string directory = GetDirectoryNameOrEmpty(filepath);
		string filename  = Path.GetFileNameWithoutExtension(filepath) ?? string.Empty;

		// Recombine the path
		return Path.Combine(directory, filename);
	}

	/// <summary>
    /// Changes the extension at the end of a filepath
    /// </summary>
    /// <param name="filepath">
    /// The filepath with the file extension to change
    /// </param>
    /// <param name="extension">
    /// The new extension
    /// </param>
    /// <returns>
    /// The filepath with the new extension
    /// </returns>
	public static string ChangeExtension(string? filepath, string? extension)
	{
		return StripExtension(filepath) + extension;
	}
}