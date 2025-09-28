using CeetemSoft.Utils;
using System.Runtime.CompilerServices;
using System.Text;

namespace CeetemSoft.Processes;

/// <summary>
/// Provides a set of simple lexical operations that can be performed on command line arguments.
/// This is in contrast to a module like Shlex which performs full shell lexical operations.
/// </summary>
public static partial class Sclex
{
	/// <summary>
    /// Gets the default seperator used for join operations
    /// </summary>
	public const string DefaultSeparator = " ";

	/// <summary>
	/// Gets the comment character
	/// </summary>
	public const char CommentChar = '#';

	private const int  MaxStackChars = 2048;

	/// <summary>
	/// Escapes a string using command line argument escaping rules
	/// </summary>
	/// <param name="input">
	/// The string to escape
	/// </param>
	/// <returns>
	/// An escaped string or empty
	/// </returns>
	/// <remarks>
	/// This function trims the input string by removing whitespace from the beginning and end of
	/// the string. If the string still contains whitespace after the trim operation, it is enclosed
	/// within quotes and finally returned.
	/// </remarks>
	public static string Escape(string? input)
	{
		return Escape(input.AsSpan());
	}

	/// <summary>
    /// Escapes a span of characters using command line argument escaping rules
    /// </summary>
    /// <param name="input">
    /// The span of characters to escape
    /// </param>
    /// <returns>
    /// An escaped string or empty
    /// </returns>
    /// <remarks>
    /// This function trims the input span by removing whitespace from the beginning and end of the
    /// the span. If the span still contains whitespace after the trim operation, it is enclosed
    /// within quotes and finally returned.
    /// </remarks>
	[SkipLocalsInit]
	public static string Escape(scoped ReadOnlySpan<char> input)
	{
		// Allocate the memory for the escape operation
		int mlen = GetEscapedMaxLength(input);
		var dest = mlen > MaxStackChars ? stackalloc char[mlen] : new char[mlen];

		// Escape the input
		input = EscapeCore(input, dest);

		// Allocate the string
		return input.IsEmpty ? string.Empty : input.ToString();
	}

	/// <summary>
    /// Splits an input span of characters into an executable and arguments using command line
    /// argument splitting rules
    /// </summary>
    /// <param name="input">
    /// The span of characters to split
    /// </param>
    /// <returns>
    /// A tuple whose first element is the executable and second element is the string of arguments
    /// following the executable. The elements will always at least contain empty strings.
    /// </returns>
	public static (string, string) SplitCommand(ReadOnlySpan<char> input)
	{
		if ((input = input.Trim()).IsEmpty)
		{
			return (string.Empty, string.Empty);
		}

		string?          executable;
		StringBuilder    text   = new();
		SpanReader<char> reader = new(input);

		// Read the executable
		if ((executable = ReadOutput(ref reader, text)) == null)
		{
			return (string.Empty, string.Empty);
		}

		// Trim the executable
		executable = executable.Trim();

		// Advance to the argument list
		if (!SkipWhitespace(ref reader))
		{
			return (executable, string.Empty);
		}

		// Construct the argument string
		return (executable, input[(reader.Index - 1)..].ToString().Trim());
	}

	/// <summary>
    /// Splits an input span of characters into individual strings using command line argument
    /// splitting rules.
    /// </summary>
    /// <param name="input">
    /// The span of characters to split
    /// </param>
    /// <returns>
    /// A an array containing all of the individual strings split from the input
    /// </returns>
    /// <remarks>
    /// This function is the inverse of the join operation. Additionally, the function is equivalent
    /// to converting a command line entered into a shell, for example, into individual arguments.
    /// </remarks>
	public static string[] Split(ReadOnlySpan<char> input)
	{
		if ((input = input.Trim()).IsEmpty)
		{
			return [];
		}

		List<string>     outputs = [];
		StringBuilder    text    = new();
		SpanReader<char> reader  = new(input);
		string?          output;

		while ((output = ReadOutput(ref reader, text)) != null)
		{
			outputs.Add(output);
			text.Clear();
		}

		return [..outputs];
	}

	/// <summary>
    /// Concatenates a span of strings into a single string using command line argument escaping
    /// rules
    /// </summary>
    /// <param name="inputs">
    /// The span of strings to concatenate into a string
    /// </param>
    /// <param name="separator">
    /// The separator to add between arguments. If null is specified <see cref="DefaultSeparator"/>
    /// is used.
    /// </param>
    /// <returns>
    /// A single string created from all of the input strings
    /// </returns>
	public static string Join(ReadOnlySpan<string?> inputs, string? separator = DefaultSeparator)
	{
		if (inputs.IsEmpty)
		{
			return string.Empty;
		}

		// Ensure we have a valid separator
		separator ??= DefaultSeparator;

		// Create a new string builder
		StringBuilder text = new();

		for (int idx = 0; idx < inputs.Length; idx++)
		{
			Join(text, inputs[idx], separator);
		}

		return text.Length > 0 ? text.ToString() : string.Empty;
	}

	/// <summary>
    /// Concatenates an array of strings into a single string using command line argument escaping
    /// rules
    /// </summary>
    /// <param name="inputs">
    /// The array of strings to concatenate into a string
    /// </param>
    /// <param name="separator">
    /// The separator to add between arguments. If null is specified <see cref="DefaultSeparator"/>
    /// is used.
    /// </param>
    /// <returns>
    /// A single string created from all of the input strings
    /// </returns>
	public static string Join(string?[]? inputs, string? separator = DefaultSeparator)
	{
		return Join(inputs.AsSpan(), separator);
	}

	/// <summary>
    /// Concatenates an enumerable of strings into a single string using command line argument
    /// escaping rules
    /// </summary>
    /// <param name="inputs">
    /// The enumerable of strings to concatenate into a string
    /// </param>
	/// <param name="separator">
    /// The separator to add between arguments. If null is specified <see cref="DefaultSeparator"/>
    /// is used.
    /// </param>
    /// <returns>
    /// A single string created from all of the input strings
    /// </returns>
	public static string Join(IEnumerable<string?>? inputs, string? separator = DefaultSeparator)
	{
		if (inputs == null)
		{
			return string.Empty;
		}

		// Ensure we have a valid separator
		separator ??= DefaultSeparator;

		// Create a new string builder
		StringBuilder text = new();

		foreach(string? input in inputs)
		{
			Join(text, input, separator);
		}

		return text.Length > 0 ? text.ToString() : string.Empty;
	}

	[SkipLocalsInit]
	private static void Join(StringBuilder text, scoped ReadOnlySpan<char> input, string separator)
	{
		// Allocate the memory for the escape operation
		int mlen = GetEscapedMaxLength(input);
		var dest = mlen > MaxStackChars ? stackalloc char[mlen] : new char[mlen];

		// Escape the input
		if ((input = EscapeCore(input, dest)).IsEmpty)
		{
			return;
		}

		// Add a separator
		else if (text.Length > 0)
		{
			text.Append(separator);
		}

		// Append the input
		text.Append(input);
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
			if (IsWhitespace(reader.Current))
			{
				continue;
			}
			else if (reader.Current == CommentChar)
			{
				SkipComment(ref reader);
				continue;
			}

			return true;
		}

		return false;
	}

	private static void SkipComment(ref SpanReader<char> reader)
	{
		while (reader.Read() != 0)
		{
			switch(reader.Current)
			{
				case '\n':
				case '\r':
					return;
				default:
					break;
			}
		}
	}

	private static ReadOnlySpan<char> EscapeCore(ReadOnlySpan<char> input, Span<char> dest)
	{
		// Trim whitespace from the input
		if ((input = input.Trim()).IsEmpty)
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