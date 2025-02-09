using CeetemSoft.Runtime;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;

namespace CeetemSoft.Io;

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
    /// <param name="position">
    /// The initial position of the reader
    /// </param>
	public ByteReader(ReadOnlySpan<byte> data, bool littleEndian = true, int position = 0)
	{
		Data         = data;
		LittleEndian = littleEndian;
		Position     = position;
		_swap        = BitConverter.IsLittleEndian != littleEndian;
	}

	/// <summary>
    /// Reads a 1 byte signed integer value at the current position of the reader. The position of
    /// the reader is incremented by 1 byte.
    /// </summary>
    /// <returns>
    /// The 1 byte signed integer value read at the current position of the reader
    /// </returns>
	public sbyte ReadInt8()
	{
		return (sbyte)Data[Position++];
	}

	/// <summary>
    /// Reads a 2 byte signed integer value at the current position of the reader. The position of
    /// the reader is incremented by 2 bytes.
    /// </summary>
    /// <returns>
    /// The 2 byte signed integer value read at the current position of the reader
    /// </returns>
	public short ReadInt16()
	{
		int position;

		position  = Position;
		Position += sizeof(short);

		return (short)ReadUint16(position);
	}

	/// <summary>
    /// Reads a 4 byte signed integer value at the current position of the reader. The position of
    /// the reader is incremented by 4 bytes.
    /// </summary>
    /// <returns>
    /// The 4 byte signed integer value read at the current position of the reader
    /// </returns>
	public int ReadInt32()
	{
		int position;

		position  = Position;
		Position += sizeof(int);

		return (int)ReadUint32(position);
	}

	/// <summary>
    /// Reads a 6 byte signed integer value at the current position of the reader. The position of
    /// the reader is incremented by 6 bytes.
    /// </summary>
    /// <returns>
    /// The 6 byte signed integer value read at the current position of the reader
    /// </returns>
	public long ReadInt48()
	{
		int position;

		position  = Position;
		Position += sizeof(int) + sizeof(short);

		return (long)ReadUint48(position);
	}

	/// <summary>
    /// Reads an 8 byte signed integer value at the current position of the reader. The position of
    /// the reader is incremented by 8 bytes.
    /// </summary>
    /// <returns>
    /// The 8 byte signed integer value read at the current position of the reader
    /// </returns>
	public long ReadInt64()
	{
		int position;

		position  = Position;
		Position += sizeof(long);

		return (long)ReadUint64(position);
	}

	/// <summary>
    /// Reads a 1 byte unsigned integer value at the current position of the reader. The position of
    /// the reader is incremented by 1 byte.
    /// </summary>
    /// <returns>
    /// The 1 byte unsigned integer value read at the current position of the reader
    /// </returns>
	public byte ReadUint8()
	{
		return Data[Position++];
	}

	/// <summary>
    /// Reads a 2 byte unsigned integer value at the current position of the reader. The position of
    /// the reader is incremented by 2 bytes.
    /// </summary>
    /// <returns>
    /// The 2 byte unsigned integer value read at the current position of the reader
    /// </returns>
	public ushort ReadUint16()
	{
		int position;

		position  = Position;
		Position += sizeof(ushort);

		return ReadUint16(position);
	}

	/// <summary>
    /// Reads a 4 byte unsigned integer value at the current position of the reader. The position of
    /// the reader is incremented by 4 bytes.
    /// </summary>
    /// <returns>
    /// The 4 byte unsigned integer value read at the current position of the reader
    /// </returns>
	public uint ReadUint32()
	{
		int position;

		position  = Position;
		Position += sizeof(uint);

		return ReadUint32(position);
	}

	/// <summary>
    /// Reads a 6 byte unsigned integer value at the current position of the reader. The position of
    /// the reader is incremented by 6 bytes.
    /// </summary>
    /// <returns>
    /// The 6 byte unsigned integer value read at the current position of the reader
    /// </returns>
	public ulong ReadUint48()
	{
		int position;

		position  = Position;
		Position += sizeof(uint) + sizeof(ushort);

		return ReadUint48(position);
	}

	/// <summary>
    /// Reads an 8 byte unsigned integer value at the current position of the reader. The position of
    /// the reader is incremented by 8 bytes.
    /// </summary>
    /// <returns>
    /// The 8 byte unsigned integer value read at the current position of the reader
    /// </returns>
	public ulong ReadUint64()
	{
		int position;

		position  = Position;
		Position += sizeof(ulong);

		return ReadUint64(position);
	}

	/// <summary>
    /// Reads a 1 byte signed integer value at the specified position within the data. The position
    /// of the reader is unchanged.
    /// </summary>
    /// <param name="position">
    /// The position within the data to read the value from
    /// </param>
    /// <returns>
    /// The 1 byte signed integer value read at the specified position within the data
    /// </returns>
	public sbyte ReadInt8(int position)
	{
		return (sbyte)Data[position];
	}

	/// <summary>
    /// Reads a 2 byte signed integer value at the specified position within the data. The position
    /// of the reader is unchanged.
    /// </summary>
    /// <param name="position">
    /// The position within the data to read the value from
    /// </param>
    /// <returns>
    /// The 2 byte signed integer value read at the specified position within the data
    /// </returns>
	[MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
	public short ReadInt16(int position)
	{
		return (short)ReadUint16(position);
	}

	/// <summary>
    /// Reads a 4 byte signed integer value at the specified position within the data. The position
    /// of the reader is unchanged.
    /// </summary>
    /// <param name="position">
    /// The position within the data to read the value from
    /// </param>
    /// <returns>
    /// The 4 byte signed integer value read at the specified position within the data
    /// </returns>
	[MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
	public int ReadInt32(int position)
	{
		return (int)ReadUint32(position);
	}

	/// <summary>
    /// Reads a 6 byte signed integer value at the specified position within the data. The position
    /// of the reader is unchanged.
    /// </summary>
    /// <param name="position">
    /// The position within the data to read the value from
    /// </param>
    /// <returns>
    /// The 6 byte signed integer value read at the specified position within the data
    /// </returns>
	[MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
	public long ReadInt48(int position)
	{
		return (long)ReadUint48(position);
	}

	/// <summary>
    /// Reads an 8 byte signed integer value at the specified position within the data. The position
    /// of the reader is unchanged.
    /// </summary>
    /// <param name="position">
    /// The position within the data to read the value from
    /// </param>
    /// <returns>
    /// The 8 byte signed integer value read at the specified position within the data
    /// </returns>
	[MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
	public long ReadInt64(int position)
	{
		return (long)ReadUint64(position);
	}

	/// <summary>
    /// Reads a 1 byte unsigned integer value at the specified position within the data. The
    /// position of the reader is unchanged.
    /// </summary>
    /// <param name="position">
    /// The position within the data to read the value from
    /// </param>
    /// <returns>
    /// The 1 byte unsigned integer value read at the specified position within the data
    /// </returns>
	public byte ReadUint8(int position)
	{
		return Data[position];
	}

	/// <summary>
    /// Reads a 2 byte unsigned integer value at the specified position within the data. The
    /// position of the reader is unchanged.
    /// </summary>
    /// <param name="position">
    /// The position within the data to read the value from
    /// </param>
    /// <returns>
    /// The 2 byte unsigned integer value read at the specified position within the data
    /// </returns>
	public ushort ReadUint16(int position)
	{
		ushort value = *(ushort*)Data.Slice(position, sizeof(ushort)).AsPointer();

		return _swap ? BinaryPrimitives.ReverseEndianness(value) : value;
	}

	/// <summary>
    /// Reads a 4 byte unsigned integer value at the specified position within the data. The
    /// position of the reader is unchanged.
    /// </summary>
    /// <param name="position">
    /// The position within the data to read the value from
    /// </param>
    /// <returns>
    /// The 4 byte unsigned integer value read at the specified position within the data
    /// </returns>
	public uint ReadUint32(int position)
	{
		uint value = *(uint*)Data.Slice(position, sizeof(uint)).AsPointer();

		return _swap ? BinaryPrimitives.ReverseEndianness(value) : value;
	}

	/// <summary>
    /// Reads a 6 byte unsigned integer value at the specified position within the data. The
    /// position of the reader is unchanged.
    /// </summary>
    /// <param name="position">
    /// The position within the data to read the value from
    /// </param>
    /// <returns>
    /// The 6 byte unsigned integer value read at the specified position within the data
    /// </returns>
	public ulong ReadUint48(int position)
	{
		uint*  data  = (uint*)Data.Slice(position, sizeof(uint) + sizeof(ushort)).AsPointer();
		uint   lower = *data;
		ushort upper = *(ushort*)(data + 1);
		ulong  value = ((ulong)upper << 32) | lower;

		return !_swap ? value :
			(BinaryPrimitives.ReverseEndianness(value) >> 16) & (long)(ulong.MaxValue >> 16);
	}

	/// <summary>
    /// Reads an 8 byte unsigned integer value at the specified position within the data. The
    /// position of the reader is unchanged.
    /// </summary>
    /// <param name="position">
    /// The position within the data to read the value from
    /// </param>
    /// <returns>
    /// The 8 byte unsigned integer value read at the specified position within the data
    /// </returns>
	public ulong ReadUint64(int position)
	{
		ulong value = *(ulong*)Data.Slice(position, sizeof(ulong)).AsPointer();

		return _swap ? BinaryPrimitives.ReverseEndianness(value) : value;
	}

	/// <summary>
    /// Implicitly converts a span to a reader
    /// </summary>
    /// <param name="span">
    /// The span to convert
    /// </param>
	public static implicit operator ByteReader(Span<byte> span)
	{
		return new(span);
	}

	/// <summary>
    /// Implicitly converts a span to a reader
    /// </summary>
    /// <param name="span">
    /// The span to convert to a reader
    /// </param>
	public static implicit operator ByteReader(ReadOnlySpan<byte> span)
	{
		return new(span);
	}

	/// <summary>
    /// Implicitly converts an array to a reader
    /// </summary>
    /// <param name="array">
    /// The array to convert to a reader
    /// </param>
	public static implicit operator ByteReader(byte[] array)
	{
		return new(array.AsSpan());
	}

	/// <summary>
    /// Gets a value indicating if the data is being read in little or big endian format
    /// </summary>
	public bool LittleEndian { get; private init; }

	/// <summary>
    /// Gets the number of bytes left to read
    /// </summary>
	public int Left => Data.Length - Position;

	/// <summary>
    /// Gets or sets the current position of the reader
    /// </summary>
	public int Position { get; set; }

	/// <summary>
    /// Gets the span of bytes being read from
    /// </summary>
	public ReadOnlySpan<byte> Data { get; private init; }
}