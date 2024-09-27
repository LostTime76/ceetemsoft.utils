using System.Runtime.CompilerServices;
using System.Text;

namespace CeetemSoft.Utils;

/// <summary>
/// Provides a set of simple lexical operations that can be performed on command line arguments.
/// This is in contrast to a module like Shlex with performs full shell lexical operations.
/// </summary>
public static partial class Sclex
{
	private const int MaxStackChars = 2048;

	/// <summary>
    /// Gets a string that is an escaped version of an input string
    /// </summary>
    /// <param name="input">
    /// The input string to escape
    /// </param>
    /// <param name="options">
    /// The options for the operation. All options except for <see cref="SclexOptions.Trim"/> are
    /// ignored.
    /// </param>
    /// <returns>
    /// The escaped string
    /// </returns>
	public static string Escape(string? input, SclexOptions options = SclexOptions.Default)
	{
		return Escape(input.AsSpan(), options);
	}

	/// <summary>
    /// Gets a string that is an escaped version of a span of characters
    /// </summary>
    /// <param name="input">
    /// The span of characters to escape
    /// </param>
    /// <param name="options">
    /// The options for the operation. All options except for <see cref="SclexOptions.Trim"/> are
    /// ignored.
    /// </param>
    /// <returns>
    /// The escaped string
    /// </returns>
	[SkipLocalsInit]
	public static string Escape(
		scoped ReadOnlySpan<char> input,
		SclexOptions options = SclexOptions.Default)
	{
		// Allocate the memory for the escape operation
		int mlen = GetEscapedMaxLength(input);
		var dest = mlen > MaxStackChars ? stackalloc char[mlen] : new char[mlen];

		// Escape the input
		input = EscapeCore(input, dest, options);

		// Allocate the string
		return input.IsEmpty ? string.Empty : input.ToString();
	}

	/// <summary>
    /// Joins all of the strings within an array seperated by a space character
    /// </summary>
    /// <param name="inputs">
    /// The array containing the strings to join
    /// </param>
    /// <param name="options">
    /// The options for the operation
    /// </param>
    /// <returns>
    /// The joined string
    /// </returns>
    /// <remarks>
    /// Strings within the array that are null or result in an empty string after trimming are not
    /// added to the output.
    /// </remarks>
	public static string Join(string?[]? inputs, SclexOptions options = SclexOptions.Default)
	{
		return Join(inputs.AsSpan(), options);
	}

	/// <summary>
    /// Joins all of the strings within an enumerable seperated by a space character
    /// </summary>
    /// <param name="inputs">
    /// The enumerable containing the strings to join
    /// </param>
    /// <param name="options">
    /// The options for the operation
    /// </param>
    /// <returns>
    /// The joined string
    /// </returns>
    /// <remarks>
    /// Strings within the enumerable that are null or result in an empty string after trimming are
    /// not added to the output.
    /// </remarks>
	public static string Join(
		IEnumerable<string?>? inputs,
		SclexOptions options = SclexOptions.Default)
	{
		if (inputs == null)
		{
			return string.Empty;
		}

		StringBuilder text = new();

		foreach (string? input in inputs)
		{
			JoinInput(text, input, options);
		}

		return text.Length > 0 ? text.ToString() : string.Empty;
	}

	/// <summary>
    /// Joins all of the strings within a span seperated by a space character
    /// </summary>
    /// <param name="inputs">
    /// The span containing the strings to join
    /// </param>
    /// <param name="options">
    /// The options for the operation
    /// </param>
    /// <returns>
    /// The joined string
    /// </returns>
    /// <remarks>
    /// Strings within the span that are null or result in an empty string after trimming are not
    /// added to the output.
    /// </remarks>
	public static string Join(
		ReadOnlySpan<string?> inputs,
		SclexOptions options = SclexOptions.Default)
	{
		StringBuilder text = new();

		for (int idx = 0; idx < inputs.Length; idx++)
		{
			JoinInput(text, inputs[idx], options);
		}

		return text.Length > 0 ? text.ToString() : string.Empty;
	}

	/// <summary>
    /// Splits a span of characters into individual strings using command line escape rules.
    /// </summary>
    /// <param name="input">
    /// The span of characters to split
    /// </param>
    /// <returns>
    /// An enumerable of strings split from the input
    /// </returns>
	public static IEnumerable<string> Split(ReadOnlySpan<char> input)
	{
		List<string>     outputs = [];
		StringBuilder    text    = new();
		SpanReader<char> reader  = new(input);
		string?          output;

		while ((output = ReadOutput(ref reader, text)) != null)
		{
			outputs.Add(output);
			text.Clear();
		}

		return outputs;
	}

	private static string? ReadOutput(ref SpanReader<char> reader, StringBuilder text)
	{
		if (!SkipWhitespace(ref reader))
		{
			return null;
		}

		while (reader.Current != 0)
		{
			if (IsWhitespace(reader.Current))
			{
				break;
			}

			switch(reader.Current)
			{
				case '"':
					AppendQuotedOutput(ref reader, text);
					break;
				default:
					AppendUnquotedOutput(ref reader, text);
					break;
			}
		}

		return text.Length > 0 ? text.ToString() : null;
	}

	private static void AppendUnquotedOutput(ref SpanReader<char> reader, StringBuilder text)
	{
		while (reader.Current != 0)
		{
			if (IsWhitespace(reader.Current))
			{
				return;
			}

			switch(reader.Current)
			{
				case '"':
					return;
				case '\\':
					AppendBackslash(reader.Read(), text);
					break;
				default:
					text.Append(reader.Current);
					break;
			}

			reader.Read();
		}

		static void AppendBackslash(char next, StringBuilder text)
		{
			if (IsWhitespace(next))
			{
				text.Append('\\');
				return;
			}

			AppendEscaped(next, text);
		}
	}

	private static void AppendQuotedOutput(ref SpanReader<char> reader, StringBuilder text)
	{
		while (reader.Read() != 0)
		{
			switch(reader.Current)
			{
				case '"':
					reader.Read();
					return;
				case '\\':
					AppendEscaped(reader.Read(), text);
					break;
				default:
					text.Append(reader.Current);
					break;
			}
		}
	}

	private static void AppendEscaped(char next, StringBuilder text)
	{
		switch(next)
		{
			case '\0':
				break;
			case '"':
				text.Append('"');
				break;
			default:
				text.Append('\\');
				text.Append(next);
				break;
		}
	}

	private static bool SkipWhitespace(ref SpanReader<char> reader)
	{
		while (reader.Read() != 0)
		{
			if (!IsWhitespace(reader.Current))
			{
				return true;
			}
		}

		return false;
	}

	[SkipLocalsInit]
	private static void JoinInput(
		StringBuilder text,
		scoped ReadOnlySpan<char> input,
		SclexOptions options)
	{
		// Escape the input
		if ((options & SclexOptions.Escape) != 0)
		{
			// Allocate the memory for the escape operation
			int mlen = GetEscapedMaxLength(input);
			var dest = mlen > MaxStackChars ? stackalloc char[mlen] : new char[mlen];

			// Escape the input
			input = EscapeCore(input, dest, options);
		}

		// Check if the input is empty
		if (input.IsEmpty)
		{
			return;
		}

		// Add a seperator
		else if (text.Length > 0)
		{
			text.Append(' ');
		}

		// Append the input to the string
		text.Append(input);
	}

	private static ReadOnlySpan<char> EscapeCore(
		ReadOnlySpan<char> input,
		Span<char> dest,
		SclexOptions options)
	{
		// Trim whitespace from the input
		if ((options & SclexOptions.Trim) != 0)
		{
			input = input.Trim();
		}
		if (input.IsEmpty)
		{
			return default;
		}

		// Add the beginning quote if we need it
		dest[0] = '"';

		// Assume initially we don't need quotes
		int  didx  = 1;
		bool quote = false;

		// Iterate through the input
		for (int idx = 0; idx < input.Length; idx++)
		{
			char curr = input[idx];

			// If the input contains whitespace, we need quotes
			quote |= IsWhitespace(curr);

			switch(curr = input[idx])
			{
				case '"':
					dest[didx++] = '\\';
					dest[didx++] = '"';
					break;
				default:
					dest[didx++] = curr;
					break;
			}
		}

		// Add the ending quote if we need it
		dest[didx] = '"';

		// Return the escaped input span
		return quote ? dest[..(didx + 1)] : dest[1..didx];
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	private static int GetEscapedMaxLength(ReadOnlySpan<char> input)
	{
		// The maximum length for an escaped input is for the degenerate case where each character
		// is a quote, which requires an additinoal escape character, plus two quotes at each end
		return (input.Length + 1) * 2;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	private static bool IsWhitespace(char value)
	{
		switch(value)
		{
			case '\v':
			case '\t':
			case '\n':
			case '\r':
			case ' ':
				return true;
			default:
				return false;
		}
	}
}