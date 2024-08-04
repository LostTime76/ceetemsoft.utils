using System.Text.RegularExpressions;

namespace CeetemSoft.Utils;

/// <summary>
/// Provides a class of extension functions for regular expressions
/// </summary>
public static class RegexExtensions
{
	/// <summary>
    /// Matches a pattern within an input span of characters
    /// </summary>
    /// <param name="pattern">
    /// The regular expression representing the pattern to match
    /// </param>
    /// <param name="input">
    /// The input span of characters to match <paramref name="pattern"/> within
    /// </param>
    /// <param name="match">
    /// The resultant match if a match is found, otherwise default
    /// </param>
    /// <returns>
    /// True if a match was found, false otherwise
    /// </returns>
    /// <exception cref="NullReferenceException">
    /// Thrown if <paramref name="pattern"/> is null
    /// </exception>
	public static bool Match(this Regex pattern, ReadOnlySpan<char> input, out ValueMatch match)
	{
		foreach (ValueMatch first in pattern.EnumerateMatches(input))
		{
			match = first;
			return true;
		}

		match = default;
		return false;
	}
}