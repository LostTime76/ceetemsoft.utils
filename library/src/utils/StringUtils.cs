namespace CeetemSoft.Utils;

/// <summary>
/// Provides a set of string utility functions
/// </summary>
public static class StringUtils
{
	/// <summary>
    /// Gets the common prefix of all the strings within a span
    /// </summary>
    /// <param name="strings">
    /// The span containing the strings to get the common prefix of
    /// </param>
    /// <returns>
    /// The common prefix of all the strings within <paramref name="strings"/> or empty if
    /// <paramref name="strings"/> is empty
    /// </returns>
	public static string GetCommonPrefix(ReadOnlySpan<string> strings)
	{
		if (strings.IsEmpty)
		{
			return string.Empty;
		}

		ReadOnlySpan<char> result = strings[0];

		for (int idx = 1; idx < strings.Length; idx++)
		{
			result = GetCommonPrefix(result, strings[idx]);
		}

		return result.ToString();
	}

	/// <summary>
    /// Gets the common prefix of all the strings within an array
    /// </summary>
    /// <param name="strings">
    /// The array containing the strings to get the common prefix of
    /// </param>
    /// <returns>
    /// The common prefix of all the strings within <paramref name="strings"/> or empty if
    /// <paramref name="strings"/> is null
    /// </returns>
	public static string GetCommonPrefix(params string[]? strings)
	{
		return GetCommonPrefix(strings.AsSpan());
	}

	/// <summary>
    /// Gets the common prefix of all the strings within an enumerable
    /// </summary>
    /// <param name="strings">
    /// The enumerable containing the strings to get the common prefix of
    /// </param>
    /// <returns>
    /// The common prefix of all the strings within <paramref name="strings"/> or empty if
    /// <paramref name="strings"/> is empty
    /// </returns>
	public static string GetCommonPrefix(IEnumerable<string>? strings)
	{
		return strings != null ? GetCommonPrefix(strings.GetEnumerator()) : string.Empty;
	}

	private static string GetCommonPrefix(IEnumerator<string> enumerator)
	{
		if (!enumerator.MoveNext())
		{
			return string.Empty;
		}

		ReadOnlySpan<char> result = enumerator.Current;

		while (enumerator.MoveNext())
		{
			result = GetCommonPrefix(result, enumerator.Current);
		}

		return result.ToString();
	}

	private static ReadOnlySpan<char> GetCommonPrefix(
		ReadOnlySpan<char> first,
		ReadOnlySpan<char> second)
	{
		int slen = Math.Min(first.Length, second.Length);

		// Find where the strings do not match
		for (int idx = 0; idx < slen; idx++)
		{
			if (first[idx] != second[idx])
			{
				slen = idx;
				break;
			}
		}

		// Update the result
		return first[..slen];
	}
}