using CeetemSoft.Runtime;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Text;

namespace CeetemSoft.Io;

/// <summary>
/// Provides a means to write primitive values to a span of bytes in little or big endian format
/// </summary>
public unsafe ref struct ByteWriter
{
	private readonly bool _swap;

	/// <summary>
    /// Creates a new writer
    /// </summary>
    /// <param name="data">
    /// The span of bytes to write to
    /// </param>
    /// <param name="littleEndian">
    /// True to write the data in little endian format, false to write the data in big endian format
    /// </param>
    /// <param name="position">
    /// The initial position of the writer
    /// </param>
	public ByteWriter(Span<byte> data, bool littleEndian = true, int position = 0)
	{
		Data         = data;
		LittleEndian = littleEndian;
		Position     = position;
		_swap        = BitConverter.IsLittleEndian != littleEndian;
	}

	/// <summary>
    /// Writes byte data at the current position of the writer. The position of the writer is
    /// incremented by the number of bytes written.
    /// </summary>
    /// <param name="data">
    /// The span containing the byte data to write
    /// </param>
	public void Write(ReadOnlySpan<byte> data)
	{
		data.CopyTo(Data[Position..]);
		Position += data.Length;
	}

	/// <summary>
    /// Writes a string value to the data at the current position of the writer. The position of
    /// the writer is incremented by the number of bytes written.
    /// </summary>
    /// <param name="value">
    /// The string value to write
    /// </param>
    /// <param name="length">
    /// The maximum number of bytes to write to the data
    /// </param>
    /// <returns>
    /// The number of bytes written to the data including a null terminator if it was written. The
    /// returned value will never exceed <paramref name="length"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="value"/> is null
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the number of utf8 encoded bytes exceeds <paramref name="length"/>
    /// </exception>
    /// <remarks>
    /// A null terminator is written to the data if the number of utf8 encoded bytes is less
    /// than <paramref name="length"/>.
    /// </remarks>
	public int WriteString(string value, int length)
	{
		return Position += WriteString(Position, value, length);
	}

	/// <summary>
    /// Writes a 1 byte signed integer value to the data at the current position of the writer. The
    /// position of the writer is incremented by 1 byte.
    /// </summary>
    /// <param name="value">
    /// The 1 byte signed integer value to write to the data
    /// </param>
	public void WriteInt8(sbyte value)
	{
		Data[Position++] = (byte)value;
	}

	/// <summary>
    /// Writes a 2 byte signed integer value to the data at the current position of the writer. The
    /// position of the writer is incremented by 2 bytes.
    /// </summary>
    /// <param name="value">
    /// The 2 byte signed integer value to write to the data
    /// </param>
	public void WriteInt16(short value)
	{
		WriteUint16(Position, (ushort)value);
		Position += sizeof(short);
	}

	/// <summary>
    /// Writes a 4 byte signed integer value to the data at the current position of the writer. The
    /// position of the writer is incremented by 4 bytes.
    /// </summary>
    /// <param name="value">
    /// The 4 byte signed integer value to write to the data
    /// </param>
	public void WriteInt32(int value)
	{
		WriteUint32(Position, (uint)value);
		Position += sizeof(int);
	}

	/// <summary>
    /// Writes a 6 byte signed integer value to the data at the current position of the writer. The
    /// position of the writer is incremented by 6 bytes.
    /// </summary>
    /// <param name="value">
    /// The 6 byte signed integer value to write to the data
    /// </param>
	public void WriteInt48(long value)
	{
		WriteUint48(Position, (ulong)value);
		Position += sizeof(int) + sizeof(short);
	}

	/// <summary>
    /// Writes a 8 byte signed integer value to the data at the current position of the writer. The
    /// position of the writer is incremented by 8 bytes.
    /// </summary>
    /// <param name="value">
    /// The 8 byte signed integer value to write to the data
    /// </param>
	public void WriteInt64(long value)
	{
		WriteUint64(Position, (ulong)value);
		Position += sizeof(long);
	}

	/// <summary>
    /// Writes a 1 byte unsigned integer value to the data at the current position of the writer.
    /// The position of the writer is incremented by 1 byte.
    /// </summary>
    /// <param name="value">
    /// The 1 byte unsigned integer value to write to the data
    /// </param>
	public void WriteUint8(byte value)
	{
		Data[Position++] = value;
	}

	/// <summary>
    /// Writes a 2 byte unsigned integer value to the data at the current position of the writer.
    /// The position of the writer is incremented by 2 bytes.
    /// </summary>
    /// <param name="value">
    /// The 2 byte unsigned integer value to write to the data
    /// </param>
	public void WriteUint16(ushort value)
	{
		WriteUint16(Position, value);
		Position += sizeof(ushort);
	}

	/// <summary>
    /// Writes a 4 byte unsigned integer value to the data at the current position of the writer.
    /// The position of the writer is incremented by 4 bytes.
    /// </summary>
    /// <param name="value">
    /// The 4 byte unsigned integer value to write to the data
    /// </param>
	public void WriteUint32(uint value)
	{
		WriteUint32(Position, value);
		Position += sizeof(uint);
	}

	/// <summary>
    /// Writes a 6 byte unsigned integer value to the data at the current position of the writer.
    /// The position of the writer is incremented by 6 bytes.
    /// </summary>
    /// <param name="value">
    /// The 6 byte unsigned integer value to write to the data
    /// </param>
	public void WriteUint48(ulong value)
	{
		WriteUint48(Position, value);
		Position += sizeof(uint) + sizeof(ushort);
	}

	/// <summary>
    /// Writes an 8 byte unsigned integer value to the data at the current position of the writer.
    /// The position of the writer is incremented by 8 bytes.
    /// </summary>
    /// <param name="value">
    /// The 8 byte unsigned integer value to write to the data
    /// </param>
	public void WriteUint64(ulong value)
	{
		WriteUint64(Position, value);
		Position += sizeof(ulong);
	}

	/// <summary>
    /// Writes byte data at the specified position within the data. The position of the writer
    /// is unchanged.
    /// </summary>
    /// <param name="position">
    /// The position within the data to write to
    /// </param>
    /// <param name="data">
    /// The span containing the byte data to write
    /// </param>
	public void Write(int position, ReadOnlySpan<byte> data)
	{
		data.CopyTo(Data[position..]);
	}

	/// <summary>
    /// Writes a 1 byte signed integer value to the data at the specified position with the data.
    /// The position of the writer is unchanged.
    /// </summary>
    /// <param name="position">
    /// The position within the data to write to
    /// </param>
    /// <param name="value">
    /// The 1 byte signed integer value to write
    /// </param>
	public void WriteInt8(int position, sbyte value)
	{
		Data[position] = (byte)value;
	}

	/// <summary>
    /// Writes a 2 byte signed integer value to the data at the specified position with the data.
    /// The position of the writer is unchanged.
    /// </summary>
    /// <param name="position">
    /// The position within the data to write to
    /// </param>
    /// <param name="value">
    /// The 2 byte signed integer value to write
    /// </param>
	[MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
	public void WriteInt16(int position, short value)
	{
		WriteUint16(position, (ushort)value);
	}

	/// <summary>
    /// Writes a 4 byte signed integer value to the data at the specified position with the data.
    /// The position of the writer is unchanged.
    /// </summary>
    /// <param name="position">
    /// The position within the data to write to
    /// </param>
    /// <param name="value">
    /// The 4 byte signed integer value to write
    /// </param>
	[MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
	public void WriteInt32(int position, int value)
	{
		WriteUint32(position, (uint)value);
	}

	/// <summary>
    /// Writes a 6 byte signed integer value to the data at the specified position with the data.
    /// The position of the writer is unchanged.
    /// </summary>
    /// <param name="position">
    /// The position within the data to write to
    /// </param>
    /// <param name="value">
    /// The 6 byte signed integer value to write
    /// </param>
	[MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
	public void WriteInt48(int position, long value)
	{
		WriteUint48(position, (ulong)value);
	}

	/// <summary>
    /// Writes a 8 byte signed integer value to the data at the specified position with the data.
    /// The position of the writer is unchanged.
    /// </summary>
    /// <param name="position">
    /// The position within the data to write to
    /// </param>
    /// <param name="value">
    /// The 8 byte signed integer value to write
    /// </param>
	[MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
	public void WriteInt64(int position, long value)
	{
		WriteUint64(position, (ulong)value);
	}

	/// <summary>
    /// Writes a 1 byte unsigned integer value to the data at the specified position with the data.
    /// The position of the writer is unchanged.
    /// </summary>
    /// <param name="position">
    /// The position within the data to write to
    /// </param>
    /// <param name="value">
    /// The 1 byte unsigned integer value to write
    /// </param>
	public void WriteUint8(int position, byte value)
	{
		Data[position] = value;
	}

	/// <summary>
    /// Writes a 2 byte unsigned integer value to the data at the specified position with the data.
    /// The position of the writer is unchanged.
    /// </summary>
    /// <param name="position">
    /// The position within the data to write to
    /// </param>
    /// <param name="value">
    /// The 2 byte unsigned integer value to write
    /// </param>
	public void WriteUint16(int position, ushort value)
	{
		value = _swap ? BinaryPrimitives.ReverseEndianness(value) : value;

		*(ushort*)Data.Slice(position, sizeof(ushort)).AsPointer() = value;
	}

	/// <summary>
    /// Writes a 4 byte unsigned integer value to the data at the specified position with the data.
    /// The position of the writer is unchanged.
    /// </summary>
    /// <param name="position">
    /// The position within the data to write to
    /// </param>
    /// <param name="value">
    /// The 4 byte unsigned integer value to write
    /// </param>
	public void WriteUint32(int position, uint value)
	{
		value = _swap ? BinaryPrimitives.ReverseEndianness(value) : value;

		*(uint*)Data.Slice(position, sizeof(uint)).AsPointer() = value;
	}

	/// <summary>
    /// Writes a 6 byte unsigned integer value to the data at the specified position with the data.
    /// The position of the writer is unchanged.
    /// </summary>
    /// <param name="position">
    /// The position within the data to write to
    /// </param>
    /// <param name="value">
    /// The 6 byte unsigned integer value to write
    /// </param>
	public void WriteUint48(int position, ulong value)
	{
		value = !_swap ? value :
			(BinaryPrimitives.ReverseEndianness(value) >> 16) & (long)(ulong.MaxValue >> 16);

		ushort* data = (ushort*)Data.Slice(position, sizeof(uint) + sizeof(ushort)).AsPointer();

		// Write the lower 2 bytes
		*data = (ushort)value;

		// Write the upper 4 bytes
		*(uint*)(data + 1) = (uint)(value >> 16);
	}

	/// <summary>
    /// Writes an 8 byte unsigned integer value to the data at the specified position with the data.
    /// The position of the writer is unchanged.
    /// </summary>
    /// <param name="position">
    /// The position within the data to write to
    /// </param>
    /// <param name="value">
    /// The 8 byte unsigned integer value to write
    /// </param>
	public void WriteUint64(int position, ulong value)
	{
		value = _swap ? BinaryPrimitives.ReverseEndianness(value) : value;

		*(ulong*)Data.Slice(position, sizeof(ulong)).AsPointer() = value;
	}

	/// <summary>
    /// Writes a string value to the data at the specified position within the data. The position
    /// of the writer is unchanged.
    /// </summary>
    /// <param name="position">
    /// The position within the data to write to
    /// </param>
    /// <param name="value">
    /// The string value to write
    /// </param>
    /// <param name="length">
    /// The maximum number of bytes to write to the data
    /// </param>
    /// <returns>
    /// The number of bytes written to the data including a null terminator if it was written. The
    /// returned value will never exceed <paramref name="length"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="value"/> is null
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the number of utf8 encoded bytes exceeds <paramref name="length"/>
    /// </exception>
    /// <remarks>
    /// A null terminator is written to the data if the number of utf8 encoded bytes is less
    /// than <paramref name="length"/>.
    /// </remarks>
	public int WriteString(int position, string value, int length)
	{
		ArgumentNullException.ThrowIfNull(value, nameof(value));

		// Encode the string value
		byte[] bytes   = Encoding.UTF8.GetBytes(value);
		int    written = bytes.Length;

		// Make sure the encoded data will fit
		if (written > length)
		{
			ThrowStringLengthExceeded();
		}

		// Write the string into the data
		bytes.AsSpan().CopyTo(Data.Slice(position, written));

		// Write a null terminator
		if (written < length)
		{
			Data[position + written++] = 0;
		}

		// Return the number of bytes written
		return written;
	}

	private static void ThrowStringLengthExceeded()
	{
		throw new InvalidOperationException("The number of encoded utf8 bytes exceeds the " +
			"maximum number of bytes to write.");
	}

	/// <summary>
    /// Implicitly converts a span to a writer
    /// </summary>
    /// <param name="span">
    /// The span to convert to a writer
    /// </param>
	public static implicit operator ByteWriter(Span<byte> span)
	{
		return new(span);
	}

	/// <summary>
    /// Implicitly converts an array to a writer
    /// </summary>
    /// <param name="array">
    /// The array to convert into a writer
    /// </param>
	public static implicit operator ByteWriter(byte[] array)
	{
		return new(array.AsSpan());
	}

	/// <summary>
    /// Gets a value indicating if the data is being written in little or big endian format
    /// </summary>
	public bool LittleEndian { get; private init; }

	/// <summary>
    /// Gets the number of bytes left to write
    /// </summary>
	public int Left => Data.Length - Position;

	/// <summary>
    /// Gets or sets the current position of the writer
    /// </summary>
	public int Position { get; set; }

	/// <summary>
    /// Gets the span of bytes being written to
    /// </summary>
	public Span<byte> Data { get; private init; }
}