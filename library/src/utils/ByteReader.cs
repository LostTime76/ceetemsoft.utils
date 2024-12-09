using System.Buffers.Binary;
using System.Runtime.CompilerServices;

namespace CeetemSoft.Utils;

/// <summary>
/// Provides a means to read primitive values from within a span of bytes in little or big endian
/// format
/// </summary>
public unsafe ref struct ByteReader
{
	private readonly bool _swap;

	/// <summary>
    /// Creates a new reader
    /// </summary>
    /// <param name="data">
    /// The span of bytes to read from
    /// </param>
    /// <param name="littleEndian">
    /// True to read the data in little endian format, false to read the data in big endian format
    /// </param>
    /// <param name="offset">
    /// The initial reader offset
    /// </param>
	public ByteReader(ReadOnlySpan<byte> data, bool littleEndian = true, int offset = 0)
	{
		Data         = data;
		LittleEndian = littleEndian;
		Offset       = offset;
		_swap        = BitConverter.IsLittleEndian != littleEndian;
	}

	/// <summary>
    /// Reads a 1 byte signed integer value at the current reader offset. The reader position is
    /// incremented by 1 byte.
    /// </summary>
    /// <returns>
    /// The 1 byte unsigned integer value read at the current reader position
    /// </returns>
	public sbyte ReadInt8()
	{
		return (sbyte)Data[Offset++];
	}

	/// <summary>
    /// Reads a 2 byte signed integer value at the current reader offset. The reader position is
    /// incremented by 2 bytes.
    /// </summary>
    /// <returns>
    /// The 2 byte signed integer value read at the current reader position
    /// </returns>
	public short ReadInt16()
	{
		int offset;

		offset  = Offset;
		Offset += sizeof(short);

		return (short)ReadUint16(offset);
	}

	/// <summary>
    /// Reads a 4 byte signed integer value at the current reader offset. The reader position is
    /// incremented by 4 bytes.
    /// </summary>
    /// <returns>
    /// The 4 byte signed integer value read at the current reader position
    /// </returns>
	public int ReadInt32()
	{
		int offset;

		offset  = Offset;
		Offset += sizeof(int);

		return (int)ReadUint32(offset);
	}

	/// <summary>
    /// Reads a 6 byte signed integer value at the current reader offset. The reader position is
    /// incremented by 6 bytes.
    /// </summary>
    /// <returns>
    /// The 6 byte signed integer value read at the current reader position
    /// </returns>
	public long ReadInt48()
	{
		int offset;

		offset  = Offset;
		Offset += sizeof(int) + sizeof(short);

		return (long)ReadUint48(offset);
	}

	/// <summary>
    /// Reads an 8 byte signed integer value at the current reader offset. The reader position is
    /// incremented by 8 bytes.
    /// </summary>
    /// <returns>
    /// The 8 byte signed integer value read at the current reader position
    /// </returns>
	public long ReadInt64()
	{
		int offset;

		offset  = Offset;
		Offset += sizeof(long);

		return (long)ReadUint64(offset);
	}

	/// <summary>
    /// Reads a 1 byte unsigned integer value at the current reader offset. The reader position is
    /// incremented by 1 byte.
    /// </summary>
    /// <returns>
    /// The 1 byte unsigned integer value read at the current reader position
    /// </returns>
	public byte ReadUint8()
	{
		return Data[Offset++];
	}

	/// <summary>
    /// Reads a 2 byte unsigned integer value at the current reader offset. The reader position is
    /// incremented by 2 bytes.
    /// </summary>
    /// <returns>
    /// The 2 byte unsigned integer value read at the current reader position
    /// </returns>
	public ushort ReadUint16()
	{
		int offset;

		offset  = Offset;
		Offset += sizeof(ushort);

		return ReadUint16(offset);
	}

	/// <summary>
    /// Reads a 4 byte unsigned integer value at the current reader offset. The reader position is
    /// incremented by 4 bytes.
    /// </summary>
    /// <returns>
    /// The 4 byte unsigned integer value read at the current reader position
    /// </returns>
	public uint ReadUint32()
	{
		int offset;

		offset  = Offset;
		Offset += sizeof(uint);

		return ReadUint32(offset);
	}

	/// <summary>
    /// Reads a 6 byte unsigned integer value at the current reader offset. The reader position is
    /// incremented by 6 bytes.
    /// </summary>
    /// <returns>
    /// The 6 byte unsigned integer value read at the current reader position
    /// </returns>
	public ulong ReadUint48()
	{
		int offset;

		offset  = Offset;
		Offset += sizeof(uint) + sizeof(ushort);

		return ReadUint48(offset);
	}

	/// <summary>
    /// Reads an 8 byte unsigned integer value at the current reader offset. The reader position is
    /// incremented by 8 bytes.
    /// </summary>
    /// <returns>
    /// The 8 byte unsigned integer value read at the current reader position
    /// </returns>
	public ulong ReadUint64()
	{
		int offset;

		offset  = Offset;
		Offset += sizeof(ulong);

		return ReadUint64(offset);
	}

	/// <summary>
    /// Reads a 1 byte signed integer value from within the data at a specified offset. The
    /// reader position is unchanged.
    /// </summary>
    /// <param name="offset">
    /// The offset within the data to read from
    /// </param>
    /// <returns>
    /// The 1 byte signed integer value read from within the data
    /// </returns>
	public sbyte ReadInt8(int offset)
	{
		return (sbyte)Data[offset];
	}

	/// <summary>
    /// Reads a 2 byte signed integer value from within the data at a specified offset. The
    /// reader position is unchanged.
    /// </summary>
    /// <param name="offset">
    /// The offset within the data to read from
    /// </param>
    /// <returns>
    /// The 2 byte signed integer value read from within the data
    /// </returns>
	[MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
	public short ReadInt16(int offset)
	{
		return (short)ReadUint16(offset);
	}

	/// <summary>
    /// Reads a 4 byte signed integer value from within the data at a specified offset. The
    /// reader position is unchanged.
    /// </summary>
    /// <param name="offset">
    /// The offset within the data to read from
    /// </param>
    /// <returns>
    /// The 4 byte signed integer value read from within the data
    /// </returns>
	[MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
	public int ReadInt32(int offset)
	{
		return (int)ReadUint32(offset);
	}

	/// <summary>
    /// Reads a 6 byte signed integer value from within the data at a specified offset. The
    /// reader position is unchanged.
    /// </summary>
    /// <param name="offset">
    /// The offset within the data to read from
    /// </param>
    /// <returns>
    /// The 6 byte signed integer value read from within the data
    /// </returns>
	[MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
	public long ReadInt48(int offset)
	{
		return (long)ReadUint48(offset);
	}

	/// <summary>
    /// Reads an 8 byte signed integer value from within the data at a specified offset. The
    /// reader position is unchanged.
    /// </summary>
    /// <param name="offset">
    /// The offset within the data to read from
    /// </param>
    /// <returns>
    /// The 8 byte signed integer value read from within the data
    /// </returns>
	[MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
	public long ReadInt64(int offset)
	{
		return (long)ReadUint64(offset);
	}

	/// <summary>
    /// Reads 1 byte unsigned integer value from within the data at a specified offset. The
    /// reader position is unchanged.
    /// </summary>
    /// <param name="offset">
    /// The offset within the data to read from
    /// </param>
    /// <returns>
    /// The 1 byte unsigned integer value read from within the data
    /// </returns>
	public byte ReadUint8(int offset)
	{
		return Data[offset];
	}

	/// <summary>
    /// Reads a 2 byte unsigned integer value from within the data at a specified offset. The
    /// reader position is unchanged.
    /// </summary>
    /// <param name="offset">
    /// The offset within the data to read from
    /// </param>
    /// <returns>
    /// The 2 byte unsigned integer value read from within the data
    /// </returns>
	public ushort ReadUint16(int offset)
	{
		ushort value = *(ushort*)Data.Slice(offset, sizeof(ushort)).AsPointer();

		return _swap ? BinaryPrimitives.ReverseEndianness(value) : value;
	}

	/// <summary>
    /// Reads a 4 byte unsigned integer value from within the data at a specified offset. The
    /// reader position is unchanged.
    /// </summary>
    /// <param name="offset">
    /// The offset within the data to read from
    /// </param>
    /// <returns>
    /// The 4 byte unsigned integer value read from within the data
    /// </returns>
	public uint ReadUint32(int offset)
	{
		uint value = *(uint*)Data.Slice(offset, sizeof(uint)).AsPointer();

		return _swap ? BinaryPrimitives.ReverseEndianness(value) : value;
	}

	/// <summary>
    /// Reads a 6 byte unsigned integer value from within the data at a specified offset. The
    /// reader position is unchanged.
    /// </summary>
    /// <param name="offset">
    /// The offset within the data to read from
    /// </param>
    /// <returns>
    /// The 6 byte unsigned integer value read from within the data
    /// </returns>
	public ulong ReadUint48(int offset)
	{
		uint*  data  = (uint*)Data.Slice(offset, sizeof(uint) + sizeof(ushort)).AsPointer();
		uint   lower = *data;
		ushort upper = *(ushort*)(data + 1);
		ulong  value = ((ulong)upper << 32) | lower;

		return !_swap ? value :
			(BinaryPrimitives.ReverseEndianness(value) >> 16) & (long)(ulong.MaxValue >> 16);
	}

	/// <summary>
    /// Reads an 8 byte unsigned integer value from within the data at a specified offset. The
    /// reader position is unchanged.
    /// </summary>
    /// <param name="offset">
    /// The offset within the data to read from
    /// </param>
    /// <returns>
    /// The 8 byte unsigned integer value read from within the data
    /// </returns>
	public ulong ReadUint64(int offset)
	{
		ulong value = *(ulong*)Data.Slice(offset, sizeof(ulong)).AsPointer();

		return _swap ? BinaryPrimitives.ReverseEndianness(value) : value;
	}

	/// <summary>
    /// Gets a value indicating if the data is being read in little or big endian format
    /// </summary>
	public bool LittleEndian { get; private init; }

	/// <summary>
    /// Gets the number of bytes left to read
    /// </summary>
	public int Left => Data.Length - Offset;

	/// <summary>
    /// Gets or sets the current reader offset
    /// </summary>
	public int Offset { get; set; }

	/// <summary>
    /// Gets the span of bytes being read from
    /// </summary>
	public ReadOnlySpan<byte> Data { get; private init; }
}