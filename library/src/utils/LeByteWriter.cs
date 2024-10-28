using System.Buffers.Binary;

namespace CeetemSoft.Utils;

/// <summary>
/// Provides a mechanism to write bytes to a span in little endian format
/// </summary>
/// <remarks>
/// The writer provides no range validation on write indicies as the application is expected to
/// manage this.
/// </remarks>
public ref struct LeByteWriter
{
	/// <summary>
    /// Creates a new writer that writes to a span
    /// </summary>
    /// <param name="data">
    /// The span to write to
    /// </param>
	public LeByteWriter(Span<byte> data)
	{
		Data = data;
	}

	/// <summary>
    /// Writes a 1 byte integer value to the span at the current writer position and increments the
    /// position by 1 byte
    /// </summary>
    /// <param name="value">
    /// The 1 byte integer value to write
    /// </param>
	public void WriteInt8(sbyte value)
	{
		Data[Index++] = (byte)value;
	}

	/// <summary>
    /// Writes a 2 byte integer value to the span at the current writer position and increments the
    /// position by 2 bytes
    /// </summary>
    /// <param name="value">
    /// The 2 byte integer value to write
    /// </param>
	public void WriteInt16(short value)
	{
		WriteInt16(Index, value);
		Index += sizeof(short);
	}

	/// <summary>
    /// Writes a 4 byte integer value to the span at the current writer position and increments the
    /// position by 4 bytes
    /// </summary>
    /// <param name="value">
    /// The 4 byte integer value to write
    /// </param>
	public void WriteInt32(int value)
	{
		WriteInt32(Index, value);
		Index += sizeof(int);
	}

	/// <summary>
    /// Writes a 6 byte integer value to the span at the current writer position and increments the
    /// position by 6 bytes
    /// </summary>
    /// <param name="value">
    /// The 6 byte integer value to write
    /// </param>
	public void WriteInt48(long value)
	{
		WriteInt48(Index, value);
		Index += sizeof(int) + sizeof(short);
	}

	/// <summary>
    /// Writes an 8 byte integer value to the span at the current writer position and increments the
    /// position by 8 bytes
    /// </summary>
    /// <param name="value">
    /// The 8 byte integer value to write
    /// </param>
	public void WriteUInt64(long value)
	{
		WriteInt64(Index, value);
		Index += sizeof(long);
	}

	/// <summary>
    /// Writes a 1 byte unsigned integer value to the span at the current writer position and
    /// increments the position by 1 byte
    /// </summary>
    /// <param name="value">
    /// The 1 byte unsigned integer value to write
    /// </param>
	public void WriteUInt8(byte value)
	{
		Data[Index++] = value;
	}

	/// <summary>
    /// Writes a 2 byte unsigned integer value to the span at the current writer position and
    /// increments the position by 2 bytes
    /// </summary>
    /// <param name="value">
    /// The 2 byte unsigned integer value to write
    /// </param>
	public void WriteUInt16(ushort value)
	{
		WriteUInt16(Index, value);
		Index += sizeof(ushort);
	}

	/// <summary>
    /// Writes a 4 byte unsigned integer value to the span at the current writer position and
    /// increments the position by 4 bytes
    /// </summary>
    /// <param name="value">
    /// The 4 byte unsigned integer value to write
    /// </param>
	public void WriteUInt32(uint value)
	{
		WriteUInt32(Index, value);
		Index += sizeof(uint);
	}

	/// <summary>
    /// Writes a 6 byte unsigned integer value to the span at the current writer position and
    /// increments the position by 6 bytes
    /// </summary>
    /// <param name="value">
    /// The 6 byte unsigned integer value to write
    /// </param>
	public void WriteUInt48(ulong value)
	{
		WriteUInt48(Index, value);
		Index += sizeof(uint) + sizeof(ushort);
	}

	/// <summary>
    /// Writes an 8 byte unsigned integer value to the span at the current writer position and
    /// increments the position by 8 bytes
    /// </summary>
    /// <param name="value">
    /// The 8 byte unsigned integer value to write
    /// </param>
	public void WriteUInt64(ulong value)
	{
		WriteUInt64(Index, value);
		Index += sizeof(ulong);
	}

	/// <summary>
    /// Writes a 1 byte integer value to the span at a specified position
    /// </summary>
    /// <param name="index">
    /// The index within the span to write the value to
    /// </param>
    /// <param name="value">
    /// The 1 byte integer value to write
    /// </param>
	public void WriteInt8(int index, sbyte value)
	{
		Data[index] = (byte)value;
	}

	/// <summary>
    /// Writes a 2 byte integer value to the span at a specified position
    /// </summary>
    /// <param name="index">
    /// The index within the span to write the value to
    /// </param>
    /// <param name="value">
    /// The 2 byte integer value to write
    /// </param>
	public void WriteInt16(int index, short value)
	{
		BinaryPrimitives.WriteInt16LittleEndian(Data.Slice(index, sizeof(short)), value);
	}

	/// <summary>
    /// Writes a 4 byte integer value to the span at a specified position
    /// </summary>
    /// <param name="index">
    /// The index within the span to write the value to
    /// </param>
    /// <param name="value">
    /// The 4 byte integer value to write
    /// </param>
	public void WriteInt32(int index, int value)
	{
		BinaryPrimitives.WriteInt32LittleEndian(Data.Slice(index, sizeof(int)), value);
	}

	/// <summary>
    /// Writes a 6 byte integer value to the span at a specified position
    /// </summary>
    /// <param name="index">
    /// The index within the span to write the value to
    /// </param>
    /// <param name="value">
    /// The 6 byte integer value to write
    /// </param>
	public void WriteInt48(int index, long value)
	{
		// Write the 4 lower bytes
		BinaryPrimitives.WriteInt32LittleEndian(Data.Slice(index, sizeof(int)), (int)value);

		// Write the 2 upper bytes
		BinaryPrimitives.WriteInt16LittleEndian(
			Data.Slice(index + sizeof(int), sizeof(short)), (short)(value >> 32));
	}

	/// <summary>
    /// Writes an 8 byte integer value to the span at a specified position
    /// </summary>
    /// <param name="index">
    /// The index within the span to write the value to
    /// </param>
    /// <param name="value">
    /// The 8 byte integer value to write
    /// </param>
	public void WriteInt64(int index, long value)
	{
		BinaryPrimitives.WriteInt64LittleEndian(Data.Slice(index, sizeof(long)), value);
	}

	/// <summary>
    /// Writes a 1 byte unsigned integer value to the span at a specified position
    /// </summary>
    /// <param name="index">
    /// The index within the span to write the value to
    /// </param>
    /// <param name="value">
    /// The 1 byte unsigned integer value to write
    /// </param>
	public void WriteUInt8(int index, byte value)
	{
		Data[index] = value;
	}

	/// <summary>
    /// Writes a 2 byte unsigned integer value to the span at a specified position
    /// </summary>
    /// <param name="index">
    /// The index within the span to write the value to
    /// </param>
    /// <param name="value">
    /// The 2 byte unsigned integer value to write
    /// </param>
	public void WriteUInt16(int index, ushort value)
	{
		BinaryPrimitives.WriteUInt16LittleEndian(Data.Slice(index, sizeof(ushort)), value);
	}

	/// <summary>
    /// Writes a 4 byte unsigned integer value to the span at a specified position
    /// </summary>
    /// <param name="index">
    /// The index within the span to write the value to
    /// </param>
    /// <param name="value">
    /// The 4 byte unsigned integer value to write
    /// </param>
	public void WriteUInt32(int index, uint value)
	{
		BinaryPrimitives.WriteUInt32LittleEndian(Data.Slice(index, sizeof(uint)), value);
	}

	/// <summary>
    /// Writes a 6 byte unsigned integer value to the span at a specified position
    /// </summary>
    /// <param name="index">
    /// The index within the span to write the value to
    /// </param>
    /// <param name="value">
    /// The 6 byte unsigned integer value to write
    /// </param>
	public void WriteUInt48(int index, ulong value)
	{
		// Write the 4 lower bytes
		BinaryPrimitives.WriteUInt32LittleEndian(Data.Slice(index, sizeof(uint)), (uint)value);

		// Write the 2 upper bytes
		BinaryPrimitives.WriteUInt16LittleEndian(
			Data.Slice(index + sizeof(uint), sizeof(ushort)), (ushort)(value >> 32));
	}

	/// <summary>
    /// Writes an 8 byte unsigned integer value to the span at a specified position
    /// </summary>
    /// <param name="index">
    /// The index within the span to write the value to
    /// </param>
    /// <param name="value">
    /// The 8 byte unsigned integer value to write
    /// </param>
	public void WriteUInt64(int index, ulong value)
	{
		BinaryPrimitives.WriteUInt64LittleEndian(Data.Slice(index, sizeof(ulong)), value);
	}

	/// <summary>
    /// Gets the number of bytes left that can be written
    /// </summary>
	public int Left => Data.Length - Index;

	/// <summary>
    /// Gets or sets current writer index
    /// </summary>
	public int Index { get; set; }

	/// <summary>
    /// Gets the span that is being written to
    /// </summary>
	public Span<byte> Data { get; private init; }
}