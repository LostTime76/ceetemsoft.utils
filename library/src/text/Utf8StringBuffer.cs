using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace CeetemSoft.Text;

/// <summary>
/// Provides a means to temporarily create and store utf8 strings often needed for native interop
/// scenarios.
/// </summary>
public sealed class Utf8StringBuffer
{
	/// <summary>
	/// The initial capacity of the buffer in bytes
	/// </summary>
	public const int InitialCapacity = 2;

	private byte[] _data;

	/// <summary>
	/// Creates a new buffer
	/// </summary>
	public Utf8StringBuffer()
	{
		Capacity = InitialCapacity;
		_data    = new byte[Capacity];
	}

	/// <summary>
	/// Resets the buffer
	/// </summary>
	public void Reset()
	{
		Used = 0;
	}

	/// <summary>
	/// Resets the buffer and allocates the first utf8 string within it
	/// </summary>
	/// <param name="text">
	/// The text to allocate the utf8 string for
	/// </param>
	/// <returns>
	/// The encoded utf8 string. If <paramref name="text"/> is empty or null, an empty utf8 string
	/// is returned. The utf8 string returned from this function is always null terminated.
	/// </returns>
	/// <remarks>
	/// The capacity of the buffer will increase to fit the encoded size of the utf8 string if
	/// the buffer capacity is too small.
	/// </remarks>
	public ReadOnlySpan<byte> Reset(string? text)
	{
		// Reset the buffer
		Used = 0;

		// Allocate the string
		return Alloc(text);
	}

	/// <summary>
	/// Allocates an additional utf8 string within the buffer
	/// </summary>
	/// <param name="text">
	/// The text to allocate the utf8 string for
	/// </param>
	/// <returns>
	/// The encoded utf8 string. If <paramref name="text"/> is empty or null, an empty utf8 string
	/// is returned. The utf8 string returned from this function is always null terminated.
	/// </returns>
	/// <remarks>
	/// The capacity of the buffer will increase to fit the encoded size of the utf8 string if
	/// the buffer capacity is too small.
	/// </remarks>
	public ReadOnlySpan<byte> Alloc(string? text)
	{
		if (string.IsNullOrEmpty(text))
		{
			return Utf8Utils.EmptyString;
		}

		// Get the number of bytes needed to encode the string
		int bytes    = Utf8Utils.GetMaxRequiredBytes(text);
		int required = bytes + Used;

		// Make sure we have enough capacity within the buffer
		if (required > Capacity)
		{
			// Increase the capacity of the buffer
			Capacity = (int)BitOperations.RoundUpToPowerOf2((nuint)required);

			// Resize the buffer
			Array.Resize(ref _data, Capacity);
		}

		// Get the destination slice for the string
		Span<byte> destination = _data.AsSpan(Used, bytes);

		// Encode the string
		Utf8Utils.Encode(text, destination);

		// Update the number of used bytes
		Used = required;

		// Return the encoded string
		return destination;
	}

	/// <summary>
	/// Gets the number of bytes currently used to encode the utf8 strings contained within the
	/// buffer
	/// </summary>
	/// <remarks>
	/// For increased performance, memory is over allocated to fit all the encoded utf8 strings
	/// contained within the buffer. Therefore this property does not match the exact minimum
	/// number of bytes needed to contain the strings.
	/// </remarks>
	public int Used { get; private set; }

	/// <summary>
	/// Gets the current capacity of the buffer
	/// </summary>
	public int Capacity { get; private set; }
}