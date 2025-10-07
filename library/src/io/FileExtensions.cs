using CeetemSoft.Utils;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;

namespace CeetemSoft.Io;

/// <summary>
/// Provides a set of file extension members
/// </summary>
public static class FileExtensions
{
	/// <summary>
	/// Provides a set of file extension members
	/// </summary>
	extension(File)
	{
		/// <summary>
		/// Writes text in utf8 encoding to a file if it does not exist or its contents differs from
		/// the text to write. Any intermediate directories are created.
		/// </summary>
		/// <param name="filepath">
		/// The path to the file to write
		/// </param>
		/// <param name="text">
		/// The text to write to the file
		/// </param>
		/// <returns>
		/// True if the file was written, false if the file already exists and its contents do not
		/// differ from the contents to be written
		/// </returns>
		/// <exception cref="ArgumentException">
		/// Thrown if <paramref name="filepath"/> is null or empty
		/// </exception>
		public static bool WriteAllTextIfDifferent(string filepath, string? text)
		{
			return WriteAllTextIfDifferent(filepath, text, Encoding.UTF8);
		}

		/// <summary>
		/// Writes text to a file if it does not exist or its contents differs from the text to
		/// write. Any intermediate directories are created.
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
		/// <returns>
		/// True if the file was written, false if the file already exists and its contents do not
		/// differ from the contents to be written
		/// </returns>
		/// <exception cref="ArgumentException">
		/// Thrown if <paramref name="filepath"/> is null or empty
		/// </exception>
		public static bool WriteAllTextIfDifferent(
			string filepath, string? text, Encoding encoding)
		{
			ArgumentException.ThrowIfNullOrEmpty(filepath, nameof(filepath));

			text ??= string.Empty;

			if (File.Exists(filepath) && (File.ReadAllText(filepath, encoding) == text))
			{
				return false;
			}

			Directory.CreateIfMissing(Path.GetDirectoryName(filepath));
			File.WriteAllText(filepath, text, encoding);
			return true;
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
			return !string.IsNullOrEmpty(filepath) && File.Exists(filepath) ?
				File.GetLastWriteTime(filepath).Ticks : -1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
		private static bool Exists(string directory, string filepath, out string resolved)
		{
			return File.Exists(resolved = Path.Combine(directory, filepath));
		}
	}
}