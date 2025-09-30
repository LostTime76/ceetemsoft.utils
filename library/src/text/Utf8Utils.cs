using System.Runtime.CompilerServices;
using System.Text;

namespace CeetemSoft.Text;

/// <summary>
/// Provides a set of utf8 utility members
/// </summary>
public static class Utf8Utils
{
	private static readonly Encoding Encoding = new UTF8Encoding(false, false);

	/// <summary>
	/// Encodes a span of characters into a null terminated utf8 string
	/// </summary>
	/// <param name="source">
	/// The span of characters to encode
	/// </param>
	/// <param name="destination">
	/// The span of bytes to store the encoding utf8 string within
	/// </param>
	/// <returns>
	/// The number of bytes written to <paramref name="destination"/> including the null
	/// terminator. If <paramref name="destination"/> is not large enough to store the encoded
	/// string, -1 is returned.
	/// </returns>
	public static int Encode(ReadOnlySpan<char> source, Span<byte> destination)
	{
		// Attempt to encode the string
		if (!Encoding.TryGetBytes(source, destination, out int written))
		{
			return -1;
		}

		// Make sure the destination buffer was large enough
		else if (written == destination.Length)
		{
			return -1;
		}

		// Set the null terminator
		destination[written] = 0;

		// Return the number of bytes that were written
		return written + 1;
	}

	/// <summary>
	/// Creates a managed string from the bytes decoded from span of bytes
	/// </summary>
	/// <param name="bytes">
	/// The span to decode
	/// </param>
	/// <returns>
	/// The managed string
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static string GetString(ReadOnlySpan<byte> bytes) => Encoding.GetString(bytes);

	/// <summary>
	/// Calculates the maximum number of bytes required to produce a utf8 string that is encoded
	/// from a span of characters
	/// </summary>
	/// <param name="chars">
	/// The span of characters to encode
	/// </param>
	/// <returns>
	/// The maximum number of bytes required to encode <paramref name="chars"/> including a null
	/// terminator.
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static int GetMaxRequiredBytes(ReadOnlySpan<char> chars) =>
		GetMaxRequiredBytes(chars.Length);

	/// <summary>
	/// Calculates the maximum number of bytes required to produce a utf8 string that is encoded
	/// from a number of characters
	/// </summary>
	/// <param name="chars">
	/// The number of characters to encode
	/// </param>
	/// <returns>
	/// The maximum number of bytes required to encode <paramref name="chars"/> including a null
	/// terminator. If <paramref name="chars"/> is negative, -1 is returned.
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static int GetMaxRequiredBytes(int chars) =>
		chars >= 1 ? (chars << 2) + 1 : chars == 0 ? 1 : -1;

	/// <summary>
	/// Gets the empty utf8 string
	/// </summary>
	public static ReadOnlySpan<byte> EmptyString => "\0"u8;
}