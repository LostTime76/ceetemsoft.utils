namespace CeetemSoft.Text;

/// <summary>
/// Provides a set of string extension members
/// </summary>
public static class StringExtensions
{
	/// <summary>
    /// Returns a string that is truncated to a maximum length
    /// </summary>
    /// <param name="input">
    /// The string to truncate
    /// </param>
    /// <param name="length">
    /// The maximum length of the string to truncate
    /// </param>
    /// <returns>
    /// If <paramref name="input"/> is null or <paramref name="length"/> is less than or equal to
    /// 0, an empty string is returned. Otherwise a truncated string is returned if the length of
    /// <paramref name="input"/> is greater than <paramref name="length"/>.
    /// </returns>
	public static string Truncate(string? input, int length)
	{
		if ((length <= 0) || (input == null))
		{
			return string.Empty;
		}
		else if (input.Length < length)
		{
			return input;
		}

		return input[..length];
	}

	/// <summary>
    /// Returns a span of characters that is truncated to a maximum length
    /// </summary>
    /// <param name="input">
    /// The span of characters to truncate
    /// </param>
    /// <param name="length">
    /// The maximum length of the span to truncate
    /// </param>
    /// <returns>
    /// If <paramref name="length"/> is less than or equal to 0, an empty span is returned.
    /// Otherwise, a truncated span is returned if the length of <paramref name="input"/> is
    /// greater than <paramref name="length"/>.
    /// </returns>
	public static ReadOnlySpan<char> Truncate(ReadOnlySpan<char> input, int length)
	{
		if (length <= 0)
		{
			return default;
		}
		else if (input.Length <= length)
		{
			return input;
		}

		return input[0..length];
	}

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