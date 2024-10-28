using System.Buffers.Binary;

namespace CeetemSoft.Utils;

/// <summary>
/// Provides a mechanism to read bytes from a span in little endian format
/// </summary>
/// <remarks>
/// The reader provides no range validation on write indicies as the application is expected to
/// manage this.
/// </remarks>
public ref struct LeByteReader
{
	/// <summary>
    /// Creates a new reader that reads from a span
    /// </summary>
    /// <param name="data">
    /// The span to read from
    /// </param>
	public LeByteReader(ReadOnlySpan<byte> data)
	{
		Data = data;
	}

	/// <summary>
    /// Reads a 1 byte integer value from the span at the current reader position and increments the
    /// position by 1 byte
    /// </summary>
    /// <returns>
    /// The 1 byte integer value read from the span
    /// </returns>
	public sbyte ReadInt8()
	{
		return (sbyte)Data[Index++];
	}

	/// <summary>
    /// Reads a 2 byte integer value from the span at the current reader position and increments the
    /// position by 2 bytes
    /// </summary>
    /// <returns>
    /// The 2 byte integer value read from the span
    /// </returns>
	public short ReadInt16()
	{
		int index;

		index  = Index;
		Index += sizeof(short);

		return ReadInt16(index);
	}

	/// <summary>
    /// Reads a 4 byte integer value from the span at the current reader position and increments the
    /// position by 4 bytes
    /// </summary>
    /// <returns>
    /// The 4 byte integer value read from the span
    /// </returns>
	public int ReadInt32()
	{
		int index;

		index  = Index;
		Index += sizeof(int);

		return ReadInt32(index);
	}

	/// <summary>
    /// Reads a 6 byte integer value from the span at the current reader position and increments the
    /// position by 6 bytes
    /// </summary>
    /// <returns>
    /// The 6 byte integer value read from the span
    /// </returns>
	public long ReadInt48()
	{
		int index;

		index  = Index;
		Index += sizeof(int) + sizeof(short);

		return ReadInt48(index);
	}

	/// <summary>
    /// Reads an 8 byte integer value from the span at the current reader position and increments
    /// the position by 8 bytes
    /// </summary>
    /// <returns>
    /// The 8 byte integer value read from the span
    /// </returns>
	public long ReadInt64()
	{
		int index;

		index  = Index;
		Index += sizeof(long);

		return ReadInt64(index);
	}

	/// <summary>
    /// Reads a 1 byte unsigned integer value from the span at the current reader position and
    /// increments the position by 1 byte
    /// </summary>
    /// <returns>
    /// The 1 byte unsigned integer value read from the span
    /// </returns>
	public byte ReadUInt8()
	{
		return Data[Index++];
	}

	/// <summary>
    /// Reads a 2 byte unsigned integer value from the span at the current reader position and
    /// increments the position by 2 bytes
    /// </summary>
    /// <returns>
    /// The 2 byte unsigned integer value read from the span
    /// </returns>
	public ushort ReadUInt16()
	{
		int index;

		index  = Index;
		Index += sizeof(ushort);

		return ReadUInt16(index);
	}

	/// <summary>
    /// Reads a 4 byte unsigned integer value from the span at the current reader position and
    /// increments the position by 4 bytes
    /// </summary>
    /// <returns>
    /// The 4 byte unsigned integer value read from the span
    /// </returns>
	public uint ReadUInt32()
	{
		int index;

		index  = Index;
		Index += sizeof(uint);

		return ReadUInt32(index);
	}

	/// <summary>
    /// Reads a 6 byte unsigned integer value from the span at the current reader position and
    /// increments the position by 6 bytes
    /// </summary>
    /// <returns>
    /// The 6 byte unsigned integer value read from the span
    /// </returns>
	public ulong ReadUInt48()
	{
		int index;

		index  = Index;
		Index += sizeof(uint) + sizeof(ushort);

		return ReadUInt48(index);
	}

	/// <summary>
    /// Reads an 8 byte unsigned integer value from the span at the current reader position and
    /// increments the position by 8 bytes
    /// </summary>
    /// <returns>
    /// The 8 byte unsigned integer value read from the span
    /// </returns>
	public ulong ReadUInt64()
	{
		int index;

		index  = Index;
		Index += sizeof(ulong);

		return ReadUInt64(index);
	}

	/// <summary>
    /// Reads a 1 byte integer value from the span at a specified position
    /// </summary>
    /// <param name="index">
    /// The index within the span to read from
    /// </param>
    /// <returns>
    /// The 1 byte integer value read from the span
    /// </returns>
	public sbyte ReadInt8(int index)
	{
		return (sbyte)Data[index];
	}

	/// <summary>
    /// Reads a 2 byte integer value from the span at a specified position
    /// </summary>
    /// <param name="index">
    /// The index within the span to read from
    /// </param>
    /// <returns>
    /// The 2 byte integer value read from the span
    /// </returns>
	public short ReadInt16(int index)
	{
		return BinaryPrimitives.ReadInt16LittleEndian(Data.Slice(index, sizeof(short)));
	}

	/// <summary>
    /// Reads a 4 byte integer value from the span at a specified position
    /// </summary>
    /// <param name="index">
    /// The index within the span to read from
    /// </param>
    /// <returns>
    /// The 4 byte integer value read from the span
    /// </returns>
	public int ReadInt32(int index)
	{
		return BinaryPrimitives.ReadInt32LittleEndian(Data.Slice(index, sizeof(int)));
	}

	/// <summary>
    /// Reads a 6 byte integer value from the span at a specified position
    /// </summary>
    /// <param name="index">
    /// The index within the span to read from
    /// </param>
    /// <returns>
    /// The 6 byte integer value read from the span
    /// </returns>
	public long ReadInt48(int index)
	{
		// Read the lower 4 bytes
		uint lower = BinaryPrimitives.ReadUInt32LittleEndian(Data.Slice(index, sizeof(int)));

		// Read the upper 2 bytes
		short upper = BinaryPrimitives.ReadInt16LittleEndian(Data.Slice(index + sizeof(int)));

		// Calculate the value
		return ((long)upper << 32) | lower;
	}

	/// <summary>
    /// Reads an 8 byte integer value from the span at a specified position
    /// </summary>
    /// <param name="index">
    /// The index within the span to read from
    /// </param>
    /// <returns>
    /// The 8 byte integer value read from the span
    /// </returns>
	public long ReadInt64(int index)
	{
		return BinaryPrimitives.ReadInt64LittleEndian(Data.Slice(index, sizeof(long)));
	}

	/// <summary>
    /// Reads a 1 byte unsigned integer value from the span at a specified position
    /// </summary>
    /// <param name="index">
    /// The index within the span to read from
    /// </param>
    /// <returns>
    /// The 1 byte unsigned integer value read from the span
    /// </returns>
	public byte ReadUInt8(int index)
	{
		return Data[index];
	}

	/// <summary>
    /// Reads a 2 byte unsigned integer value from the span at a specified position
    /// </summary>
    /// <param name="index">
    /// The index within the span to read from
    /// </param>
    /// <returns>
    /// The 2 byte unsigned integer value read from the span
    /// </returns>
	public ushort ReadUInt16(int index)
	{
		return BinaryPrimitives.ReadUInt16LittleEndian(Data.Slice(index, sizeof(short)));
	}

	/// <summary>
    /// Reads a 4 byte unsigned integer value from the span at a specified position
    /// </summary>
    /// <param name="index">
    /// The index within the span to read from
    /// </param>
    /// <returns>
    /// The 4 byte unsigned integer value read from the span
    /// </returns>
	public uint ReadUInt32(int index)
	{
		return BinaryPrimitives.ReadUInt32LittleEndian(Data.Slice(index, sizeof(int)));
	}

	/// <summary>
    /// Reads a 6 byte unsigned integer value from the span at a specified position
    /// </summary>
    /// <param name="index">
    /// The index within the span to read from
    /// </param>
    /// <returns>
    /// The 6 byte unsigned integer value read from the span
    /// </returns>
	public ulong ReadUInt48(int index)
	{
		// Read the lower 4 bytes
		uint lower = BinaryPrimitives.ReadUInt32LittleEndian(Data.Slice(index, sizeof(int)));

		// Read the upper 2 bytes
		ushort upper = BinaryPrimitives.ReadUInt16LittleEndian(Data.Slice(index + sizeof(int)));

		// Calculate the value
		return ((ulong)upper << 32) | lower;
	}

	/// <summary>
    /// Reads an 8 byte unsigned integer value from the span at a specified position
    /// </summary>
    /// <param name="index">
    /// The index within the span to read from
    /// </param>
    /// <returns>
    /// The 8 byte unsigned integer value read from the span
    /// </returns>
	public ulong ReadUInt64(int index)
	{
		return BinaryPrimitives.ReadUInt64LittleEndian(Data.Slice(index, sizeof(long)));
	}

	/// <summary>
    /// Gets the number of bytes left to read within the span
    /// </summary>
	public int Left => Data.Length - Index;

	/// <summary>
	/// Gets or sets current reader position
	/// </summary>
	public int Index { get; set; }

	/// <summary>
	/// Gets the span that is being read from
	/// </summary>
	public ReadOnlySpan<byte> Data { get; private init; }
}