using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace CeetemSoft.Utils;

/// <summary>
/// Provides a set of json utility functions
/// </summary>
public static class JsonUtils
{
	/// <summary>
    /// Attempts to deserialize json within a file
    /// </summary>
    /// <typeparam name="T">
    /// The generic type of the class to deserialize
    /// </typeparam>
    /// <param name="filepath">
    /// The path to the file containing the json to deserialize
    /// </param>
    /// <param name="type">
    /// The json type information for the deserialization operation
    /// </param>
    /// <param name="value">
    /// The deserialized json value
    /// </param>
    /// <returns>
    /// True if the deserialzation was successful, false if the file does not exist, the
    /// deserialization failed, or if the result value of the deserialization is null
    /// </returns>
	public static bool TryDeserialize<T>(
		[NotNullWhen(true)]string filepath,
		JsonTypeInfo<T> type,
		[NotNullWhen(true)]out T? value)
	{
		try
		{
			value = JsonSerializer.Deserialize(File.ReadAllText(filepath), type);
			return value != null;
		}
		catch
		{
			value = default;
			return false;
		}
	}
}