using CeetemSoft.Collections;
using CeetemSoft.Runtime;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace CeetemSoft.Native;

/// <summary>
/// Provides a set of native utf8 utility members
/// </summary>
unsafe public static class NativeUtf8
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	internal static readonly Encoding Encoding = new UTF8Encoding(false, false);

	/// <summary>
    /// Allocates a null terminated utf8 string that is produced by encoding a managed string
	/// </summary>
	/// <param name="str">
	/// The managed string to encode
	/// </param>
	/// <returns>
	/// A pointer to the allocated null terminated utf8 string
	/// </returns>
	/// <exception cref="OutOfMemoryException"/>
	/// <remarks>
    /// This function uses unmanaged memory for the allocation that must manually be freed using the
    /// <see cref="NativeMemory.Free(void*)"/> function.
	/// </remarks>
	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static byte* Alloc(string? str)
	{
		return str != null ? Alloc(str.AsSpan(), out _) : null;
	}

	/// <summary>
    /// Allocates a null terminated utf8 string that is produced by encoding a span of characters
	/// </summary>
	/// <param name="chars">
	/// The span of characters to encode
	/// </param>
	/// <returns>
	/// A pointer to the allocated null terminated utf8 string
	/// </returns>
	/// <exception cref="OutOfMemoryException"/>
	/// <remarks>
    /// This function uses unmanaged memory for the allocation that must manually be freed using the
    /// <see cref="NativeMemory.Free(void*)"/> function.
	/// </remarks>
	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static byte* Alloc(ReadOnlySpan<char> chars)
	{
		return Alloc(chars, out _);
	}

	/// <summary>
    /// Allocates a null terminated utf8 string that is produced by encoding a managed string
	/// </summary>
	/// <param name="str">
	/// The managed string to encode
	/// </param>
	/// <param name="bytes">
	/// The number of bytes within the allocated utf8 string including the null terminator
	/// </param>
	/// <returns>
	/// A pointer to the allocated null terminated utf8 string
	/// </returns>
	/// <exception cref="OutOfMemoryException"/>
	/// <remarks>
    /// This function uses unmanaged memory for the allocation that must manually be freed using the
    /// <see cref="NativeMemory.Free(void*)"/> function.
	/// </remarks>
	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static byte* Alloc(string? str, out int bytes)
	{
		if (str == null)
		{
			bytes = 0;
			return null;
		}

		return Alloc(str.AsSpan(), out bytes);
	}

	/// <summary>
    /// Allocates a null terminated utf8 string that is produced by encoding a span of characters
	/// </summary>
	/// <param name="chars">
	/// The span of characters to encode
	/// </param>
	/// <param name="bytes">
	/// The number of bytes within the allocated utf8 string including the null terminator
	/// </param>
	/// <returns>
	/// A pointer to the allocated null terminated utf8 string
	/// </returns>
	/// <exception cref="OutOfMemoryException"/>
	/// <remarks>
    /// This function uses unmanaged memory for the allocation that must manually be freed using the
    /// <see cref="NativeMemory.Free(void*)"/> function.
	/// </remarks>
	public static byte* Alloc(ReadOnlySpan<char> chars, out int bytes)
	{
		// Count the number of bytes required for the string
		bytes = GetMaxRequiredBytes(chars);

		// Allocate the memory for the string
		byte* dst = (byte*)NativeMemory.Alloc((nuint)bytes);

		// Encode the string
		bytes = Encode(chars, new(dst, bytes));

		// Return the string
		return dst;
	}

	/// <summary>
	/// Allocates a native array of pointers to null terminated utf8 strings. The utf8 strings are
    /// encoded from the managed strings within a span.
	/// </summary>
	/// <param name="span">
	/// The span containing the strings to encode
	/// </param>
	/// <returns>
	/// A pointer the allocated native array. The memory must be freed using the
    /// <see cref="NativeMemory.Free(void*)"/> function. If <paramref name="span"/> is empty, no
    /// array is allocated and null is returned.
	/// </returns>
	public static byte** Alloc(ReadOnlySpan<string?> span)
	{
		if (span.IsEmpty)
		{
			return null;
		}

		int dbytes = 0;
		int length = span.Length;
		int pbytes = sizeof(byte*) * length;

		// Count the number of bytes required for all the strings
		for (int idx = 0; idx < length; idx++)
		{
			dbytes += GetRequiredBytes(span[idx]);
		}

		byte** array  = (byte**)NativeMemory.Alloc((nuint)(pbytes + dbytes));
		byte*  buffer = (byte*)((long)array + pbytes);

		// Encode the strings
		for (int idx = 0, enc = 0; idx < length; idx++)
		{
			// Create the destination buffer
			var dst = new Span<byte>(buffer + enc, dbytes - enc);

			// Encode the string
			enc += Encode(span[idx], dst);

			// Set the string pointer
			array[idx] = dst.AsPointer();
		}

		return array;
	}

	/// <summary>
    /// Encodes a span of characters into a null terminated utf8 string
    /// </summary>
    /// <param name="chars">
    /// The span of characters to encode
    /// </param>
    /// <param name="bytes">
    /// The span of bytes to store the encoded utf8 string within
    /// </param>
    /// <returns>
    /// The number of bytes written to <paramref name="bytes"/> including the null terminator. If
    /// <paramref name="bytes"/> is not large enough to store the encoded bytes, -1 is returned.
    /// </returns>
	public static int Encode(ReadOnlySpan<char> chars, Span<byte> bytes)
	{
		// Encode the chars into the destination
		if (!Encoding.TryGetBytes(chars, bytes, out int written) || (bytes.Length == written))
		{
			// Operation failed
			return -1;
		}

		// Set the null terminator
		bytes[written] = 0;

		// Return the number of bytes that were written
		return written + 1;
	}

	/// <summary>
	/// Gets the number of bytes within a null terminated utf8 string
	/// </summary>
	/// <param name="str">
	/// A pointer to a null terminated utf8 string
	/// </param>
	/// <returns>
	/// The number of bytes within <paramref name="str"/>. If <paramref name="str"/> is null, --1
    /// is returned.
	/// </returns>
	public static int GetLength(byte* str)
	{
		int length;

		if (str == null)
		{
			return -1;
		}

		for (length = 0; str[length] != 0; length++);

		return length;
	}

	/// <summary>
	/// Gets the the number of bytes within a utf8 string
	/// </summary>
	/// <param name="str">
	/// A pointer to a utf8 string
	/// </param>
	/// <param name="maxBytes">
	/// The maximum number of bytes that should be counted within <paramref name="str"/>
	/// </param>
	/// <returns>
    /// The number of bytes within <paramref name="str"/> up to <paramref name="maxBytes"/> not
    /// including the null terminator. If <paramref name="str"/> is null or
    /// <paramref name="maxBytes"/> is negative, -1 is returned.
	/// </returns>
	public static int GetLength(byte* str, int maxBytes)
	{
		int length;

		if ((str == null) || (maxBytes < 0))
		{
			return -1;
		}

		for (length = 0; (length < maxBytes) && (str[length] != 0); length++);

		return length;
	}

	/// <summary>
    /// Creates a managed string from the bytes decoded from a span of bytes
    /// </summary>
    /// <param name="bytes">
    /// The span of bytes to decode
    /// </param>
    /// <returns>
    /// A managed string created from decoding <paramref name="bytes"/>
    /// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static string GetString(ReadOnlySpan<byte> bytes)
	{
		return Encoding.GetString(bytes);
	}

	/// <summary>
    /// Creates a managed string from the bytes decoded from a null terminated utf8 string
	/// </summary>
	/// <param name="str">
	/// A pointer to a null terminated utf8 string to decode
	/// </param>
	/// <returns>
    /// A managed string created from decoding <paramref name="str"/>. If <paramref name="str"/> is
    /// null, null is returned.
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static string? GetString(byte* str)
	{
		return GetString(str, GetLength(str));
	}

	/// <summary>
    /// Creates a managed string produced from decoding a null terminated utf8 string
	/// </summary>
	/// <param name="str">
	/// A pointer to a utf8 string to decode
	/// </param>
	/// <param name="bytes">
	/// The number of bytes within <paramref name="str"/> to decode not including the null
	/// terminator
	/// </param>
	/// <returns>
    /// The managed string. If <paramref name="str"/> is null or <paramref name="bytes"/> is less
    /// than or equal to 0, null is returned.
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static string? GetString(byte* str, int bytes)
	{
		return (str != null) && (bytes > 0) ? Encoding.GetString(str, bytes) : null;
	}

	/// <summary>
    /// Creates a managed string array produced by decoding a number of null terminated utf8 strings
    /// within a native array
    /// </summary>
    /// <param name="array">
	/// The native array containing the strings to decode
	/// </param>
    /// <param name="length">
    /// The number of strings within the array to decode
    /// </param>
    /// <returns>
    /// The managed string array. If <paramref name="array"/> is null or <paramref name="length"/>
    /// is less than 0, null is returned.
    /// </returns>
	public static string[]? GetArray(byte** array, int length)
	{
		if ((array == null) || (length < 0))
		{
			return null;
		}

		string[] decoded = new string[length];

		for (int idx = 0; idx < length; idx++)
		{
			decoded[idx] = GetString(array[idx])!;
		}

		return decoded;
	}

	/// <summary>
    /// Calculates the exact number of bytes required to produce a utf8 string that is encoded from
    /// a managed string
    /// </summary>
    /// <param name="str">
    /// The managed string to encode
    /// </param>
    /// <returns>
    /// The number of bytes required to encode <paramref name="str"/> including a null terminator.
    /// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static int GetRequiredBytes(string? str)
	{
		return str != null ? Encoding.GetByteCount(str) + 1 : 1;
	}

	/// <summary>
    /// Calculates the exact number of bytes required to produce a utf8 string that is encoded from
    /// a span of characters
    /// </summary>
    /// <param name="chars">
    /// The span of characters to encode
    /// </param>
    /// <returns>
    /// The number of bytes required to encode <paramref name="chars"/> including a null
    /// terminator
    /// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static int GetRequiredBytes(ReadOnlySpan<char> chars)
	{
		return !chars.IsEmpty ? Encoding.GetByteCount(chars) + 1 : 1;
	}

	/// <summary>
    /// Calculates an over estimation of the number of bytes required to produce a utf8 string that
    /// is encoded from a number of characters
    /// </summary>
    /// <param name="chars">
    /// The number of characters to encode
    /// </param>
    /// <returns>
    /// The over estimated number of bytes required to encode <paramref name="chars"/> characters
    /// including a null terminator. If <paramref name="chars"/> is negative, -1 is returned.
    /// </returns>
    /// <remarks>
    /// This function is faster to execute than calculating the exact number of bytes required for
    /// the encode operation but over estimates.
    /// </remarks>
	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static int GetMaxRequiredBytes(int chars)
	{
		return chars >= 1 ? chars << 2 : chars == 0 ? 1 : -1;
	}

	/// <summary>
	/// Calculates an over estimation of the number of bytes required to produce a utf8 string
    /// that is encoded from a managed string
	/// </summary>
	/// <param name="str">
	/// The managed string to encode
	/// </param>
	/// <returns>
	/// The over estimated number of bytes required to encode <paramref name="str"/> including a
    /// null terminator
	/// </returns>
	/// <remarks>
    /// This function is faster to execute than calculating the exact number of bytes required for
    /// the encode operation but over estimates.
    /// </remarks>
	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static int GetMaxRequiredBytes(string? str)
	{
		return str != null ? GetMaxRequiredBytes(str.Length) : 1;
	}

	/// <summary>
	/// Calculates an over estimation of the number of bytes required to produce a utf8 string
    /// that is encoded from a span of characters
	/// </summary>
	/// <param name="chars">
	/// The span of characters to encode
	/// </param>
	/// <returns>
	/// The over estimated number of bytes required to encode <paramref name="chars"/> including a
    /// null terminator
	/// </returns>
	/// <remarks>
    /// This function is faster to execute than calculating the exact number of bytes required for
    /// the encode operation but over estimates.
    /// </remarks>
	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static int GetMaxRequiredBytes(ReadOnlySpan<char> chars)
	{
		return GetMaxRequiredBytes(chars.Length);
	}

	/// <summary>
    /// Determines if a utf8 string is empty
    /// </summary>
    /// <param name="str">
    /// A pointer to a null terminated utf8 string to test
    /// </param>
    /// <returns>
    /// True if <paramref name="str"/> is not null and is also empty
    /// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static bool IsEmpty(byte* str)
	{
		return (str != null) && (str[0] == 0);
	}

	/// <summary>
    /// Determines if a utf8 string is null or empty
    /// </summary>
    /// <param name="str">
    /// A pointer to a null terminated utf8 string to test
    /// </param>
    /// <returns>
    /// True if <paramref name="str"/> is null or empty, false otherwise
    /// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static bool IsNullOrEmpty(byte* str)
	{
		return (str == null) || (str[0] == 0);
	}
}