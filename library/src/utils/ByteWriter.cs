using System.Buffers.Binary;
using System.Runtime.CompilerServices;

namespace CeetemSoft.Utils;

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
    /// <param name="offset">
    /// The initial writer offset
    /// </param>
	public ByteWriter(Span<byte> data, bool littleEndian = true, int offset = 0)
	{
		Data         = data;
		LittleEndian = littleEndian;
		Offset       = offset;
		_swap        = BitConverter.IsLittleEndian != littleEndian;
	}

	/// <summary>
    /// Writes a 1 byte signed integer value to the data at the current writer offset. The
    /// writer position is incremented by 1 byte.
    /// </summary>
    /// <param name="value">
    /// The 1 byte signed integer value to write
    /// </param>
	public void WriteInt8(sbyte value)
	{
		Data[Offset++] = (byte)value;
	}

	/// <summary>
    /// Writes a 2 byte signed integer value to the data at the current writer offset. The
    /// writer position is incremented by 2 bytes.
    /// </summary>
    /// <param name="value">
    /// The 2 byte signed integer value to write
    /// </param>
	public void WriteInt16(short value)
	{
		WriteUint16(Offset, (ushort)value);
		Offset += sizeof(short);
	}

	/// <summary>
    /// Writes a 4 byte signed integer value to the data at the current writer offset. The
    /// writer position is incremented by 4 bytes.
    /// </summary>
    /// <param name="value">
    /// The 4 byte signed integer value to write
    /// </param>
	public void WriteInt32(int value)
	{
		WriteUint32(Offset, (uint)value);
		Offset += sizeof(int);
	}

	/// <summary>
    /// Writes a 6 byte signed integer value to the data at the current writer offset. The
    /// writer position is incremented by 6 bytes.
    /// </summary>
    /// <param name="value">
    /// The 6 byte signed integer value to write
    /// </param>
	public void WriteInt48(long value)
	{
		WriteUint48(Offset, (ulong)value);
		Offset += sizeof(int) + sizeof(short);
	}

	/// <summary>
    /// Writes an 8 byte signed integer value to the data at the current writer offset. The
    /// writer position is incremented by 8 bytes.
    /// </summary>
    /// <param name="value">
    /// The 8 byte signed integer value to write
    /// </param>
	public void WriteInt64(long value)
	{
		WriteUint64(Offset, (ulong)value);
		Offset += sizeof(long);
	}

	/// <summary>
    /// Writes a 1 byte unsigned integer value to the data at the current writer offset. The
    /// writer position is incremented by 1 byte.
    /// </summary>
    /// <param name="value">
    /// The 1 byte unsigned integer value to write
    /// </param>
	public void WriteUint8(byte value)
	{
		Data[Offset++] = value;
	}

	/// <summary>
    /// Writes a 2 byte unsigned integer value to the data at the current writer offset. The
    /// writer position is incremented by 2 bytes.
    /// </summary>
    /// <param name="value">
    /// The 2 byte unsigned integer value to write
    /// </param>
	public void WriteUint16(ushort value)
	{
		WriteUint16(Offset, value);
		Offset += sizeof(ushort);
	}

	/// <summary>
    /// Writes a 4 byte unsigned integer value to the data at the current writer offset. The
    /// writer position is incremented by 4 bytes.
    /// </summary>
    /// <param name="value">
    /// The 4 byte unsigned integer value to write
    /// </param>
	public void WriteUint32(uint value)
	{
		WriteUint32(Offset, value);
		Offset += sizeof(uint);
	}

	/// <summary>
    /// Writes a 6 byte unsigned integer value to the data at the current writer offset. The
    /// writer position is incremented by 6 bytes.
    /// </summary>
    /// <param name="value">
    /// The 6 byte unsigned integer value to write
    /// </param>
	public void WriteUint48(ulong value)
	{
		WriteUint48(Offset, value);
		Offset += sizeof(uint) + sizeof(ushort);
	}

	/// <summary>
    /// Writes an 8 byte unsigned integer value to the data at the current writer offset. The
    /// writer position is incremented by 8 bytes.
    /// </summary>
    /// <param name="value">
    /// The 8 byte unsigned integer value to write
    /// </param>
	public void WriteUint64(ulong value)
	{
		WriteUint64(Offset, value);
		Offset += sizeof(ulong);
	}

	/// <summary>
    /// Writes a 1 byte signed integer value to the data at the specified offset. The writer
    /// position is unchanged.
    /// </summary>
    /// <param name="offset">
    /// The offset within the data to write to
    /// </param>
    /// <param name="value">
    /// The 1 byte signed integer value to write
    /// </param>
	public void WriteInt8(int offset, sbyte value)
	{
		Data[offset] = (byte)value;
	}

	/// <summary>
    /// Writes a 2 byte signed integer value to the data at the specified offset. The writer
    /// position is unchanged.
    /// </summary>
    /// <param name="offset">
    /// The offset within the data to write to
    /// </param>
    /// <param name="value">
    /// The 2 byte signed integer value to write
    /// </param>
	[MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
	public void WriteInt16(int offset, short value)
	{
		WriteUint16(offset, (ushort)value);
	}

	/// <summary>
    /// Writes a 4 byte signed integer value to the data at the specified offset. The writer
    /// position is unchanged.
    /// </summary>
    /// <param name="offset">
    /// The offset within the data to write to
    /// </param>
    /// <param name="value">
    /// The 4 byte signed integer value to write
    /// </param>
	[MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
	public void WriteInt32(int offset, int value)
	{
		WriteUint32(offset, (uint)value);
	}

	/// <summary>
    /// Writes a 6 byte signed integer value to the data at the specified offset. The writer
    /// position is unchanged.
    /// </summary>
    /// <param name="offset">
    /// The offset within the data to write to
    /// </param>
    /// <param name="value">
    /// The 6 byte signed integer value to write
    /// </param>
	[MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
	public void WriteInt48(int offset, long value)
	{
		WriteUint48(offset, (ulong)value);
	}

	/// <summary>
    /// Writes an 8 byte signed integer value to the data at the specified offset. The writer
    /// position is unchanged.
    /// </summary>
    /// <param name="offset">
    /// The offset within the data to write to
    /// </param>
    /// <param name="value">
    /// The 8 byte signed integer value to write
    /// </param>
	[MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
	public void WriteInt64(int offset, long value)
	{
		WriteUint64(offset, (ulong)value);
	}

	/// <summary>
    /// Writes a 1 byte unsigned integer value to the data at the specified offset. The writer
    /// position is unchanged.
    /// </summary>
    /// <param name="offset">
    /// The offset within the data to write to
    /// </param>
    /// <param name="value">
    /// The 1 byte unsigned integer value to write
    /// </param>
	public void WriteUint8(int offset, byte value)
	{
		Data[offset] = value;
	}

	/// <summary>
    /// Writes a 2 byte unsigned integer value to the data at the specified offset. The writer
    /// position is unchanged.
    /// </summary>
    /// <param name="offset">
    /// The offset within the data to write to
    /// </param>
    /// <param name="value">
    /// The 2 byte unsigned integer value to write
    /// </param>
	public void WriteUint16(int offset, ushort value)
	{
		value = _swap ? BinaryPrimitives.ReverseEndianness(value) : value;

		*(ushort*)Data.Slice(offset, sizeof(ushort)).AsPointer() = value;
	}

	/// <summary>
    /// Writes a 4 byte unsigned integer value to the data at the specified offset. The writer
    /// position is unchanged.
    /// </summary>
    /// <param name="offset">
    /// The offset within the data to write to
    /// </param>
    /// <param name="value">
    /// The 4 byte unsigned integer value to write
    /// </param>
	public void WriteUint32(int offset, uint value)
	{
		value = _swap ? BinaryPrimitives.ReverseEndianness(value) : value;

		*(uint*)Data.Slice(offset, sizeof(uint)).AsPointer() = value;
	}

	/// <summary>
    /// Writes a 6 byte unsigned integer value to the data at the specified offset. The writer
    /// position is unchanged.
    /// </summary>
    /// <param name="offset">
    /// The offset within the data to write to
    /// </param>
    /// <param name="value">
    /// The 6 byte unsigned integer value to write
    /// </param>
	public void WriteUint48(int offset, ulong value)
	{
		value = !_swap ? value :
			(BinaryPrimitives.ReverseEndianness(value) >> 16) & (long)(ulong.MaxValue >> 16);

		ushort* data = (ushort*)Data.Slice(offset, sizeof(uint) + sizeof(ushort)).AsPointer();

		// Write the lower 2 bytes
		*data = (ushort)value;

		// Write the upper 4 bytes
		*(uint*)(data + 1) = (uint)(value >> 16);
	}

	/// <summary>
    /// Writes an 8 byte unsigned integer value to the data at the specified offset. The writer
    /// position is unchanged.
    /// </summary>
    /// <param name="offset">
    /// The offset within the data to write to
    /// </param>
    /// <param name="value">
    /// The 8 byte unsigned integer value to write
    /// </param>
	public void WriteUint64(int offset, ulong value)
	{
		value = _swap ? BinaryPrimitives.ReverseEndianness(value) : value;

		*(ulong*)Data.Slice(offset, sizeof(ulong)).AsPointer() = value;
	}

	/// <summary>
    /// Gets a value indicating if the data is being written in little or big endian format
    /// </summary>
	public bool LittleEndian { get; private init; }

	/// <summary>
    /// Gets the number of bytes left to write
    /// </summary>
	public int Left => Data.Length - Offset;

	/// <summary>
    /// Gets or sets the current writer offset
    /// </summary>
	public int Offset { get; set; }

	/// <summary>
    /// Gets the span of bytes being written to
    /// </summary>
	public Span<byte> Data { get; private init; }
}