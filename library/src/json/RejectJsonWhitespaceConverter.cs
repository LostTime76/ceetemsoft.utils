using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CeetemSoft.Json;

/// <summary>
/// Provides a json converter that rejects reading and writing of strings that are null or contain
/// only whitespace
/// </summary>
public class RejectJsonWhitespaceConverter : JsonConverter<string>
{
	/// <summary>
	/// Reads a string value
	/// </summary>
	/// <param name="reader">
	/// The reader
	/// </param>
	/// <param name="type">
	/// The type to convert the read value to
	/// </param>
	/// <param name="options">
	/// The reader options
	/// </param>
	/// <returns>
	/// The string value
	/// </returns>
	/// <exception cref="JsonException">
	/// Thrown if the value that was read is null or contains only whitespace
	/// </exception>
	public override string Read(ref Utf8JsonReader reader, Type type,
		JsonSerializerOptions options)
	{
		string? value = reader.GetString();

		if (string.IsNullOrWhiteSpace(value))
		{
			ThrowInvalidDeserialization();
		}

		return value;
	}

	/// <summary>
	/// Writes a string value
	/// </summary>
	/// <param name="writer">
	/// The writer
	/// </param>
	/// <param name="value">
	/// The value to write
	/// </param>
	/// <param name="options">
	/// The writer options
	/// </param>
	/// <exception cref="JsonException">
	/// Thrown if <paramref name="value"/> is null or contains only whitespace
	/// </exception>
	public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
	{
		if (string.IsNullOrWhiteSpace(value))
		{
			ThrowInvalidSerialization();
		}

		writer.WriteStringValue(value);
	}

	[DoesNotReturn]
	private static void ThrowInvalidSerialization()
	{
		throw new JsonException("The value cannot be null or contain only whitespace.");
	}

	[DoesNotReturn]
	private static void ThrowInvalidDeserialization()
	{
		throw new JsonException("The string cannot be null or contain only whitespace.");
	}
}