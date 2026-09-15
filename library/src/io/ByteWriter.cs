using System.Text;

namespace CeetemSoft.Io;

public ref struct ByteWriter
{
	public ByteWriter(Span<byte> data)
	{
		Data = data;
	}

	public void Write(Range range, ReadOnlySpan<byte> bytes)
	{
		var (offset, total) = range.GetOffsetAndLength(Data.Length);

		// Truncate the bytes up to the end of the range
		var length = Math.Min(bytes.Length, total);

		// Write the first part of the data
		bytes[0..length].CopyTo(Data.Slice(offset, length));

		// Clear the rest of the bytes
		Data.Slice(offset + length, total).Clear();
	}

	public void WriteString(Range range, string? value)
		=> Write(range, Encoding.UTF8.GetBytes(value ?? string.Empty));

	public Span<byte> Data { get; private init; }
}