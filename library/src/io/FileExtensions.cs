using System.Text;

namespace CeetemSoft.Io;

/// <summary>
/// Provides a set of <see cref="File"/> extension members
/// </summary>
public static class FileExtensions
{
	extension(File)
	{
		/// <summary>
		/// Writes data to a file is the contents of the file are different than the data to write.
		/// Any intermediate directories to the file are automatically created
		/// </summary>
		/// <param name="filepath">
		/// The path of the file to write to
		/// </param>
		/// <param name="data">
		/// The data to write to the file
		/// </param>
		/// <exception cref="ArgumentException">
		/// Thrown if <paramref name="filepath"/> is null, empty, or only contains whitespace
		/// characters
		/// </exception>
		public static void WriteAllBytesIfDifferent(string filepath, ReadOnlySpan<byte> data)
		{
			ArgumentException.ThrowIfNullOrWhiteSpace(filepath, nameof(filepath));

			var directory = Path.GetDirectoryName(filepath);

			if (!string.IsNullOrEmpty(directory))
			{
				Directory.CreateDirectory(directory);
			}
			else if (File.Exists(filepath) && (File.ReadAllBytes(filepath) == data))
			{
				return;
			}

			File.WriteAllBytes(filepath, data);
		}

		/// <summary>
		/// Writes text to a file if the contents of the file are different than the text to write.
		/// Any intermediate directories to the file are automatically created.
		/// </summary>
		/// <param name="filepath">
		/// The path of the file to write to
		/// </param>
		/// <param name="text">
		/// The text to write to the file
		/// </param>
		/// <param name="encoding">
		/// The encoding of the text to write. If no encoding is provided, utf8 is used.
		/// </param>
		/// <exception cref="ArgumentException">
		/// Thrown if <paramref name="filepath"/> is null, empty, or only contains whitespace
		/// characters
		/// </exception>
		public static void WriteAllTextIfDifferent(
			string filepath, string? text = null, Encoding? encoding = null)
		{
			ArgumentException.ThrowIfNullOrWhiteSpace(filepath, nameof(filepath));

			text     ??= string.Empty;
			encoding ??= Encoding.UTF8;

			var directory = Path.GetDirectoryName(filepath);

			if (!string.IsNullOrEmpty(directory))
			{
				Directory.CreateDirectory(directory);
			}
			if (File.Exists(filepath) && (File.ReadAllText(filepath, encoding) == text))
			{
				return;
			}

			File.WriteAllText(filepath, text, encoding);
		}
	}
}