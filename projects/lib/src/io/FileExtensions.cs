using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace CeetemSoft.Io;

/// <summary>
/// Provides a set of file extension members
/// </summary>
public static class FileExtensions
{
	/// <summary>
    /// Writes text to a file if it does not exist or its contents differs than the text to write
    /// </summary>
    /// <param name="filepath">
    /// The path to the file to write
    /// </param>
    /// <param name="text">
    /// The text to write to the file
    /// </param>
	/// <remarks>
    /// This function will touch the file to update the last modified time if the text is not
    /// written to the file
    /// </remarks>
	public static void WriteAllTextIfDifferent(string filepath, string? text)
	{
		WriteAllTextIfDifferent(filepath, text, Encoding.UTF8);
	}

	/// <summary>
    /// Writes text to a file if it does not exist or its contents differs than the text to write
    /// </summary>
    /// <param name="filepath">
    /// The path to the file to write
    /// </param>
    /// <param name="text">
    /// The text to write to the file
    /// </param>
    /// <param name="encoding">
    /// The encoding of the text to write
    /// </param>
    /// <remarks>
    /// This function will touch the file to update the last modified time if the text is not
    /// written to the file
    /// </remarks>
	public static void WriteAllTextIfDifferent(string filepath, string? text, Encoding encoding)
	{
		ArgumentException.ThrowIfNullOrEmpty(filepath);

		// Make sure there is some text
		text ??= string.Empty;

		// Only write the file if we have to
		if (File.Exists(filepath) && (File.ReadAllText(filepath, encoding) == text))
		{
			File.SetLastWriteTime(filepath, DateTime.Now);
			return;
		}

		File.WriteAllText(filepath, text, encoding);
	}

	/// <summary>
    /// Gets a value that indicates if a file is newer than another file
    /// </summary>
    /// <param name="filepath">
    /// The path of the file
    /// </param>
    /// <param name="timestamp">
    /// The timestamp of the other file to compare to
    /// </param>
    /// <returns>
    /// True if the file exists and the timestamp of the file is greater than or equal to
    /// <paramref name="timestamp"/>, otherwise false
    /// </returns>
	public static bool IsNewer([NotNullWhen(true)]string? filepath, long timestamp)
	{
		long otimestamp;

		return (otimestamp = GetTimestamp(filepath)) >= 0 && (otimestamp > timestamp);
	}

	/// <summary>
    /// Gets a value that indicates if a file is older than another file
    /// </summary>
    /// <param name="filepath">
    /// The path of the file
    /// </param>
    /// <param name="timestamp">
    /// The timestamp of the other file to compare to
    /// </param>
    /// <returns>
    /// True if the file does not exist or its timestamp is less than <paramref name="timestamp"/>,
    /// otherwise false
    /// </returns>
	public static bool IsOlder([NotNullWhen(false)]string? filepath, long timestamp)
	{
		long otimestamp;

		return ((otimestamp = GetTimestamp(filepath)) < 0) || (timestamp > otimestamp);
	}

	/// <summary>
    /// Gets the last write timestamp of a file
    /// </summary>
    /// <param name="filepath">
    /// The path to the file to get the timestamp of
    /// </param>
    /// <returns>
    /// A positive value if the timestamp was retrieved successfully, otherwise -1
    /// </returns>
	public static long GetTimestamp(string? filepath)
	{
		return File.Exists(filepath) ? File.GetLastWriteTime(filepath).Ticks : -1;
	}
}