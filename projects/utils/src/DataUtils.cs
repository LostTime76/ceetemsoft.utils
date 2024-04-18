using System.Buffers.Binary;
using System.Runtime.CompilerServices;

namespace CeetemSoft.Utils;

/// <summary>
/// Provides a class of utility functions for bit operations
/// </summary>
unsafe public static class DataUtils
{
	/// <summary>
    /// Reverses the endianness of a 2 byte signed integer
    /// </summary>
    /// <param name="value">
    /// The 2 byte signed integer value to reverse
    /// </param>
    /// <returns>
    /// The reversed 2 byte signed integer
    /// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static short Rev16(short value)
	{
		return BinaryPrimitives.ReverseEndianness(value);
	}

	/// <summary>
    /// Reverses the endianness of a 2 byte unsigned integer
    /// </summary>
    /// <param name="value">
    /// The 2 byte unsigned integer value to reverse
    /// </param>
    /// <returns>
    /// The reversed 2 byte unsigned integer
    /// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static ushort Rev16(ushort value)
	{
		return BinaryPrimitives.ReverseEndianness(value);
	}

	/// <summary>
    /// Reverses the endianness of each 2 byte word within a 4 byte signed integer
    /// </summary>
    /// <param name="value">
    /// The 4 byte signed integer to reverse
    /// </param>
    /// <returns>
    /// The reversed 4 byte signed integer
    /// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static int Rev16(int value)
	{
		int upper = BinaryPrimitives.ReverseEndianness((short)(value >> 16));
		int lower = BinaryPrimitives.ReverseEndianness((short)value);
		return (upper << 16) | lower;
	}

	/// <summary>
    /// Reverses the endianness of each 2 byte word within a 4 byte unsigned integer
    /// </summary>
    /// <param name="value">
    /// The 4 byte unsigned integer to reverse
    /// </param>
    /// <returns>
    /// The reversed 4 byte unsigned integer
    /// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static uint Rev16(uint value)
	{
		uint upper = BinaryPrimitives.ReverseEndianness((ushort)(value >> 16));
		uint lower = BinaryPrimitives.ReverseEndianness((ushort)value);
		return (upper << 16) | lower;
	}

	/// <summary>
    /// Reverses the endianness of each 2 byte word within an 8 byte signed integer
    /// </summary>
    /// <param name="value">
    /// The 8 byte signed integer to reverse
    /// </param>
    /// <returns>
    /// The reversed 8 byte signed integer
    /// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static long Rev16(long value)
	{
		long upper = Rev16((int)(value >> 32));
		long lower = Rev16((int)value);
		return (upper << 32) | lower;
	}

	/// <summary>
    /// Reverses the endianness of each 2 byte word within an 8 byte unsigned integer
    /// </summary>
    /// <param name="value">
    /// The 8 byte unsigned integer to reverse
    /// </param>
    /// <returns>
    /// The reversed 8 byte unsigned integer
    /// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static ulong Rev16(ulong value)
	{
		ulong upper = Rev16((uint)(value >> 32));
		ulong lower = Rev16((uint)value);
		return (upper << 32) | lower;
	}

	/// <summary>
    /// Reverses the endianness of a 4 byte signed integer
    /// </summary>
    /// <param name="value">
    /// The 4 byte signed integer to reverse
    /// </param>
    /// <returns>
    /// The reversed 4 byte signed integer
    /// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static int Rev32(int value)
	{
		return BinaryPrimitives.ReverseEndianness(value);
	}

	/// <summary>
    /// Reverses the endianness of a 4 byte unsigned integer
    /// </summary>
    /// <param name="value">
    /// The 4 byte unsigned integer to reverse
    /// </param>
    /// <returns>
    /// The reversed 4 byte unsigned integer
    /// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static uint Rev32(uint value)
	{
		return BinaryPrimitives.ReverseEndianness(value);
	}

	/// <summary>
    /// Reverses the endianness of each 4 byte word within an 8 byte signed integer
    /// </summary>
    /// <param name="value">
    /// The 8 byte signed integer to reverse
    /// </param>
    /// <returns>
    /// The reversed 8 byte signed integer
    /// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static long Rev32(long value)
	{
		long upper = Rev32((int)(value >> 32));
		long lower = Rev32((int)value);
		return (upper << 32) | lower;
	}

	/// <summary>
    /// Reverses the endianness of each 4 byte word within an 8 byte unsigned integer
    /// </summary>
    /// <param name="value">
    /// The 8 byte unsigned integer to reverse
    /// </param>
    /// <returns>
    /// The reversed 8 byte unsigned integer
    /// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static ulong Rev32(ulong value)
	{
		ulong upper = Rev32((uint)(value >> 32));
		ulong lower = Rev32((uint)value);
		return (upper << 32) | lower;
	}

	/// <summary>
    /// Reverses the endianness of an 8 byte signed integer
    /// </summary>
    /// <param name="value">
    /// The 8 byte signed integer to reverse
    /// </param>
    /// <returns>
    /// The reversed 8 byte signed integer
    /// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static long Rev64(long value)
	{
		return BinaryPrimitives.ReverseEndianness(value);
	}

	/// <summary>
    /// Reverses the endianness of an 8 byte unsigned integer
    /// </summary>
    /// <param name="value">
    /// The 8 byte unsigned integer to reverse
    /// </param>
    /// <returns>
    /// The reversed 8 byte unsigned integer
    /// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static ulong Rev64(ulong value)
	{
		return BinaryPrimitives.ReverseEndianness(value);
	}

	/// <summary>
    /// Reverses the endianness of each 2 byte word within a span of bytes
    /// </summary>
    /// <param name="data">
    /// The span of bytes containing the data to reverse
    /// </param>
	public static void Rev16(Span<byte> data)
	{
		short* src = (short*)data.AsPointer();

		for (int word = 0, words = data.Length >> 1; word < words; word++)
		{
			src[word] = BinaryPrimitives.ReverseEndianness(src[word]);
		}
	}

	/// <summary>
    /// Reverses the endianness of each 4 byte word within a span of bytes
    /// </summary>
    /// <param name="data">
    /// The span of bytes containing the data to reverse
    /// </param>
	public static void Rev32(Span<byte> data)
	{
		int* src = (int*)data.AsPointer();

		for (int word = 0, words = data.Length >> 2; word < words; word++)
		{
			src[word] = BinaryPrimitives.ReverseEndianness(src[word]);
		}
	}

	/// <summary>
    /// Reverses the endianness of each 8 byte word within a span of bytes
    /// </summary>
    /// <param name="data">
    /// The span of bytes containing the data to reverse
    /// </param>
	public static void Rev64(Span<byte> data)
	{
		long* src = (long*)data.AsPointer();

		for (int word = 0, words = data.Length >> 2; word < words; word++)
		{
			src[word] = BinaryPrimitives.ReverseEndianness(src[word]);
		}
	}

	/// <summary>
    /// Reads a 2 byte integer value in little endian format
    /// </summary>
    /// <param name="value">
    /// The value to read
    /// </param>
    /// <returns>
    /// The value read in little endian format
    /// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static short ReadLe(short value)
	{
		return BitConverter.IsLittleEndian ? value : BinaryPrimitives.ReverseEndianness(value);
	}

	/// <summary>
    /// Reads a 4 byte integer value in little endian format
    /// </summary>
    /// <param name="value">
    /// The value to read
    /// </param>
    /// <returns>
    /// The value read in little endian format
    /// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static int ReadLe(int value)
	{
		return BitConverter.IsLittleEndian ? value : BinaryPrimitives.ReverseEndianness(value);
	}

	/// <summary>
    /// Reads an 8 byte integer value in little endian format
    /// </summary>
    /// <param name="value">
    /// The value to read
    /// </param>
    /// <returns>
    /// The value read in little endian format
    /// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static long ReadLe(long value)
	{
		return BitConverter.IsLittleEndian ? value : BinaryPrimitives.ReverseEndianness(value);
	}

	/// <summary>
    /// Reads a 2 byte unsigned integer value in little endian format
    /// </summary>
    /// <param name="value">
    /// The value to read
    /// </param>
    /// <returns>
    /// The value read in little endian format
    /// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static ushort ReadLe(ushort value)
	{
		return BitConverter.IsLittleEndian ? value : BinaryPrimitives.ReverseEndianness(value);
	}

	/// <summary>
    /// Reads a 4 byte unsigned integer value in little endian format
    /// </summary>
    /// <param name="value">
    /// The value to read
    /// </param>
    /// <returns>
    /// The value read in little endian format
    /// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static uint ReadLe(uint value)
	{
		return BitConverter.IsLittleEndian ? value : BinaryPrimitives.ReverseEndianness(value);
	}

	/// <summary>
    /// Reads an 8 byte unsigned integer value in little endian format
    /// </summary>
    /// <param name="value">
    /// The value to read
    /// </param>
    /// <returns>
    /// The value read in little endian format
    /// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static ulong ReadLe(ulong value)
	{
		return BitConverter.IsLittleEndian ? value : BinaryPrimitives.ReverseEndianness(value);
	}
}