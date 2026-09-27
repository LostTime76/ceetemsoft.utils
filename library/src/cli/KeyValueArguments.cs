using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace CeetemSoft.Cli;

/// <summary>
/// Provides a means to parse a set of command line arguments into a table of key, value pairs
/// </summary>
public sealed class KeyValueArguments : IReadOnlyDictionary<string, string>
{
	private readonly Dictionary<string, string> _table;

	private static readonly char[] Separators = [':', '='];

	/// <summary>
	/// Creates new arguments
	/// </summary>
	/// <param name="arguments">
	/// The arguments to parse
	/// </param>
	/// <exception cref="ArgumentException">
	/// Thrown if there was an issue parsing the arguments
	/// </exception>
	public KeyValueArguments(ReadOnlySpan<string> arguments)
	{
		_table = CreateTable(arguments);
	}

	/// <summary>
	/// Gets a value that indicates if an argument is within the table
	/// </summary>
	/// <param name="argument">
	/// The argument to test
	/// </param>
	/// <returns>
	/// True if the argument is within the table, false otherwise
	/// </returns>
	public bool ContainsKey(string argument) => _table.ContainsKey(argument);

	/// <summary>
	/// Retrieves a string value for an argument
	/// </summary>
	/// <param name="argument">
	/// The argument to get the value of
	/// </param>
	/// <param name="value">
	/// The value of the argument
	/// </param>
	/// <returns>
	/// True if the value for the argument was retrieved successfully, false otherwise
	/// </returns>
	public bool TryGetValue(string argument, [MaybeNullWhen(false)] out string value)
		=> _table.TryGetValue(argument, out value);

	/// <summary>
	/// Retrieves a string value for an argument
	/// </summary>
	/// <param name="argument">
	/// The argument to get the value of
	/// </param>
	/// <returns>
	/// The value of the argument
	/// </returns>
	/// <exception cref="ArgumentException">
	/// Thrown if <paramref name="argument"/> is null or contains only whitespace
	/// </exception>
	/// <exception cref="ArgumentException">
	/// Thrown if the argument value is not within the table
	/// </exception>
	public string GetString(string argument)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(argument, nameof(argument));

		if (!_table.TryGetValue(argument, out var value))
		{
			ThrowExpectedArgument(argument);
		}

		return value;
	}

	/// <summary>
	/// Retrieves a string value for an argument
	/// </summary>
	/// <param name="argument">
	/// The argument to get the value of
	/// </param>
	/// <returns>
	/// The value of the argument or null if the value is not within the table
	/// </returns>
	/// <exception cref="ArgumentException">
	/// Thrown if <paramref name="argument"/> is null or contains only whitespace
	/// </exception>
	public string? GetStringOrNull(string argument)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(argument, nameof(argument));

		return _table.TryGetValue(argument, out var value) ? value : null;
	}

	/// <summary>
	/// Retrieves an enum value for an argument
	/// </summary>
	/// <typeparam name="T">
	/// The type of the enum
	/// </typeparam>
	/// <param name="argument">
	/// The argument to get the value of
	/// </param>
	/// <returns>
	/// The value of the argument
	/// </returns>
	/// <exception cref="ArgumentException">
	/// Thrown if <paramref name="argument"/> is null or contains only whitespace
	/// </exception>
	/// <exception cref="ArgumentException">
	/// Thrown if the argument value is not within the table or if the value is not valid for the
	/// given enum
	/// </exception>
	public T GetEnum<T>(string argument) where T: struct, Enum
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(argument, nameof(argument));

		T result = default;

		if (!_table.TryGetValue(argument, out var value))
		{
			ThrowExpectedArgument(argument);
		}
		else if (!Enum.TryParse(value, true, out result))
		{
			ThrowInvalidEnumValue(argument, value);
		}

		return result;
	}

	/// <summary>
	/// Retrieves an enum value for an argument
	/// </summary>
	/// <typeparam name="T">
	/// The type of the enum
	/// </typeparam>
	/// <param name="argument">
	/// The argument to get the value of
	/// </param>
	/// <returns>
	/// The value of the argument or the default value of the enum if the value is not within the
	/// table or not a valid value for the enum
	/// </returns>
	/// <exception cref="ArgumentException">
	/// Thrown if <paramref name="argument"/> is null or contains only whitespace
	/// </exception>
	public T GetEnumOrDefault<T>(string argument) where T: struct, Enum
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(argument, nameof(argument));

		if (!_table.TryGetValue(argument, out var value))
		{
			return default;
		}

		return Enum.TryParse(value, true, out T result) ? result : default;
	}

	/// <summary>
	/// Gets an enumerator for the table
	/// </summary>
	/// <returns>
	/// An enumerator for the table
	/// </returns>
	public IEnumerator<KeyValuePair<string, string>> GetEnumerator() => _table.GetEnumerator();

	/// <summary>
	/// Gets an enumerator for the table
	/// </summary>
	/// <returns>
	/// An enumerator for the table
	/// </returns>
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	private static Dictionary<string, string> CreateTable(ReadOnlySpan<string?> arguments)
	{
		var table = new Dictionary<string, string>(arguments.Length);

		foreach(string? argument in arguments)
		{
			if (!ParseKeyValuePair(argument, out var key, out var value))
			{
				continue;
			}
			else if (!table.TryAdd(key, value))
			{
				ThrowDuplicateArgument(key);
			}
		}

		return table;
	}

	private static bool ParseKeyValuePair(string? argument,
		[NotNullWhen(true)]out string? key, [NotNullWhen(true)]out string? value)
	{
		if (string.IsNullOrWhiteSpace(argument))
		{
			key   = null;
			value = null;
			return false;
		}

		var tokens = argument.Split(Separators, StringSplitOptions.RemoveEmptyEntries);

		if (tokens.Length != 2)
		{
			ThrowInvalidKeyValueFormat(argument);
		}

		key   = tokens[0].Trim();
		value = tokens[1].Trim();

		if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(value))
		{
			ThrowInvalidKeyValueFormat(argument);
		}

		return true;
	}

	[DoesNotReturn]
	private static void ThrowExpectedArgument(string argument)
		=> throw new ArgumentException($"Expected a value for the '{argument}' argument.");

	private static void ThrowInvalidEnumValue(string argument, string value)
		=> throw new ArgumentException($"'{value}' is not a valid value for '{argument}'.");

	[DoesNotReturn]
	private static void ThrowInvalidKeyValueFormat(string argument)
		=> throw new ArgumentException($"'{argument}' is not in a valid key, value format.");

	[DoesNotReturn]
	private static void ThrowDuplicateArgument(string argument)
		=> throw new ArgumentException($"Duplicate value for the '{argument}' argument.");

	/// <summary>
	/// Gets the number of arguments within the table
	/// </summary>
	public int Count => _table.Count;

	/// <summary>
	/// Retrieves a string value from an argument
	/// </summary>
	/// <param name="argument">
	/// The argument to get the value of
	/// </param>
	/// <returns>
	/// The value of the argument
	/// </returns>
	public string this[string argument] => _table[argument];

	/// <summary>
	/// Gets the arguments within the table
	/// </summary>
	public IEnumerable<string> Keys => _table.Keys;

	/// <summary>
	/// Gets the values within the table
	/// </summary>
	public IEnumerable<string> Values => _table.Values;
}