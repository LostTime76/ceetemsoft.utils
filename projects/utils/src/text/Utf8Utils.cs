using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace CeetemSoft.Utils.Text;

/// <summary>
/// Provides a class of utf8 utility functions for interop scenarios
/// </summary>
unsafe public static class Utf8Utils
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static readonly Encoding _encoding = new UTF8Encoding(true, false);

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
	/// Allocates a native array of pointers to null terminated utf8 strings encoded from the
	/// strings within a managed array
	/// </summary>
	/// <param name="strings">
	/// The managed array containing the strings to encode
	/// </param>
	/// <returns>
	/// A pointer to the array of pointers to null terminated utf8 strings The memory must be
    /// freed using the <see cref="NativeMemory.Free(void*)"/> function.
	/// </returns>
	/// <exception cref="ArgumentOutOfRangeException"/>
	/// <exception cref="OutOfMemoryException"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static byte** Alloc(string[]? strings)
	{
		return Alloc(new ReadOnlySpan<string>(strings));
	}

	/// <summary>
	/// Allocates a native array of pointers to null terminated utf8 strings encoded from the
	/// strings within a span of strings
	/// </summary>
	/// <param name="strings">
	/// The span of strings containing the strings to encode
	/// </param>
	/// <returns>
	/// A pointer to the array of pointers to null terminated utf8 strings. The memory must be
    /// freed using the <see cref="NativeMemory.Free(void*)"/> function.
	/// </returns>
	/// <exception cref="OutOfMemoryException"/>
	public static byte** Alloc(ReadOnlySpan<string> strings)
	{
		int bytes    = 0;
		int items    = strings.Length;
		int arrBytes = sizeof(byte*) * items;

		// Count the number of bytes required for all the strings
		for (int idx = 0; idx < items; idx++)
		{
			bytes += GetRequiredBytes(strings[idx]);
		}

		// Allocate the memory for the array
		byte** array   = (byte**)NativeMemory.Alloc((nuint)(arrBytes + bytes));
		byte*  strBuff = (byte*)((long)array + arrBytes);

		// Iterate through all the strings to encode
		for (int idx = 0, enc = 0; idx < items; idx++)
		{
			// Create the destination buffer
			Span<byte> dst = new(strBuff + enc, bytes - enc);

			// Encode the string
			enc += Encode(strings[idx], dst);

			// Set the string pointer
			array[idx] = dst.AsPointer();
		}

		// Return the array
		return array;
	}

	/// <summary>
    /// Encodes a managed string into a null terminated utf8 string
    /// </summary>
    /// <param name="str">
    /// The managed string to encode
    /// </param>
    /// <param name="bytes">
    /// The span of bytes to store the encoded utf8 string wtihin
    /// </param>
    /// <returns>
    /// The number of bytes written to <paramref name="bytes"/> including the null terminator. If
    /// <paramref name="str"/> is null or <paramref name="bytes"/> is not large enough to store
    /// the encoded bytes, -1 is returned
    /// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static int Encode(string? str, Span<byte> bytes)
	{
		return str != null ? Encode(str.AsSpan(), bytes) : -1;
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
		if (!_encoding.TryGetBytes(chars, bytes, out int written) || (bytes.Length == written))
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
    /// Calculates the hash code of a null terminated utf8 span of bytes
    /// </summary>
    /// <param name="bytes">
    /// The null terminated utf8 span of bytes to calculate the hash code of
    /// </param>
    /// <returns>
    /// The calculated hash code of <paramref name="bytes"/>
    /// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static int GetHashCode(ReadOnlySpan<byte> bytes)
	{
		return GetHashCode(bytes.AsPointer(), bytes.Length);
	}

	/// <summary>
    /// Calculates the hash code of a null terminated utf8 string
    /// </summary>
    /// <param name="str">
    /// A pointer to a null terminated utf8 string to calculate the hash code of
    /// </param>
    /// <returns>
    /// The calculated hash code of <paramref name="str"/>. If <paramref name="str"/> is null, 0
    /// is returned.
    /// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static int GetHashCode(byte* str)
	{
		return GetHashCode(str, GetLength(str));
	}

	/// <summary>
    /// Calculates the hash code of a utf8 string
    /// </summary>
    /// <param name="str">
    /// A pointer to the utf8 string to calculate the hash code of
    /// </param>
    /// <param name="bytes">
    /// The number of bytes to hash within <paramref name="str"/>
    /// </param>
    /// <returns>
    /// The calculated hash code of <paramref name="str"/>. If <paramref name="str"/> is null or
    /// <paramref name="bytes"/> is negative, 0 is returned.
    /// </returns>
	public static int GetHashCode(byte* str, int bytes)
	{
		int hash = 5381;

		if ((str == null) || (bytes < 0))
		{
			return 0;
		}
		while (bytes-- > 0)
		{
			hash = (hash * 33) ^ str[bytes];
		}

		return hash;
	}

	/// <summary>
    /// Determines if two null terminated utf8 strings are equal
    /// </summary>
    /// <param name="str1">
    /// A pointer to the first null terminated utf8 string to compare
    /// </param>
    /// <param name="str2">
    /// A pointer to the second null terminated utf8 string to compare
    /// </param>
    /// <returns>
    /// True if <paramref name="str1"/> is equal to <paramref name="str2"/>, false otherwise
    /// </returns>
	public static bool CompareEqual(byte* str1, byte* str2)
	{
		int len;

		// If the lengths of the strings do not match, they can't be equal
		if ((len = GetLength(str1)) != GetLength(str2))
		{
			// Lengths do not match
			return false;
		}

		// Are both strings null?
		else if (len == -1)
		{
			// Both strings are null
			return true;
		}

		// Both strings are the same length, so we need to compare bytes
		while (len-- > 0)
		{
			if (str1[len] != str2[len]) { return false; }
		}

		// Both strings are equal
		return true;
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
    /// Gets the number of bytes within a utf8 span of bytes
    /// </summary>
    /// <param name="str">
    /// The utf8 encoded span of bytes to read
    /// </param>
    /// <returns>
    /// The number of bytes within <paramref name="str"/> up to the length of
    /// <paramref name="str"/> not including the null terminator
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static int GetLength(ReadOnlySpan<byte> str)
	{
		return GetLength(str.AsPointer(), str.Length);
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
		return _encoding.GetString(bytes);
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
    /// Creates a managed string from the bytes decoded from a pointer to a utf8 string
	/// </summary>
	/// <param name="str">
	/// A pointer to a utf8 string to decode
	/// </param>
	/// <param name="bytes">
	/// The number of bytes within <paramref name="str"/> to decode not including the null
	/// terminator
	/// </param>
	/// <returns>
    /// A managed string decoded from <paramref name="str"/>. If <paramref name="str"/> is null or
    /// <paramref name="bytes"/> is negative, null is returned.
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static string? GetString(byte* str, int bytes)
	{
		return (str != null) && (bytes >= 0) ? _encoding.GetString(str, bytes) : null;
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
    /// If <paramref name="str"/> is null, 0 is returned.
    /// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static int GetRequiredBytes(string? str)
	{
		return str != null ? _encoding.GetByteCount(str) + 1 : 0;
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
		return !chars.IsEmpty ? _encoding.GetByteCount(chars) + 1 : 1;
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
    /// null terminator. If <paramref name="str"/> is null, -1 is returned.
	/// </returns>
	/// <remarks>
    /// This function is faster to execute than calculating the exact number of bytes required for
    /// the encode operation using the <see cref="GetRequiredBytes(string?)"/> function but over
    /// estimates.
    /// </remarks>
	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static int GetMaxRequiredBytes(string? str)
	{
		return str != null ? GetMaxRequiredBytes(str.Length) : -1;
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
    /// the encode operation using the <see cref="GetRequiredBytes(ReadOnlySpan{char})"/> function
    /// but over estimates.
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