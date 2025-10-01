using System.Runtime.CompilerServices;

namespace CeetemSoft.Io;

/// <summary>
/// Provides a set of path extension members
/// </summary>
public static class PathExtensions
{
	extension(Path)
	{
		/// <summary>
		/// Gets the filepath of the calling source code file at compile time
		/// </summary>
		/// <param name="source">
		/// The filepath of the calling source code file, filled in by the compiler
		/// </param>
		/// <returns>
		/// The filepath of the calling source code file
		/// </returns>
		/// <remarks>
		/// The caller must invoke this function without any parameters for proper operation.
		/// </remarks>
		public static string GetSourceFilepath([CallerFilePath] string? source = null)
		{
			return source!;
		}

		/// <summary>
		/// Gets the directory of the calling source code file at compile time
		/// </summary>
		/// <param name="source">
		/// The filepath of the calling source code file, filled in by the compiler
		/// </param>
		/// <returns>
		/// The directory of the calling source code file
		/// </returns>
		/// <remarks>
		/// The caller must invoke this function without any parameters for proper operation
		/// </remarks>
		public static string GetSourceDirectory([CallerFilePath] string? source = null)
		{
			return Path.GetDirectoryName(source)!;
		}
	}
}