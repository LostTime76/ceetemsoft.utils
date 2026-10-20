using System.Buffers.Binary;
using System.Text;

namespace CeetemSoft.Utils;

/// <summary>
/// Provides a set of span extension members
/// </summary>
public static class SpanExtensions
{
	private static readonly Encoder _encoder = Encoding.UTF8.GetEncoder();

	extension(Span<byte> span)
	{
		/// <summary>
		/// Writes byte data to a span
		/// </summary>
		/// <param name="data">
		/// The data to write to the span
		/// </param>
		/// <returns>
		/// The shorter length of <paramref name="span"/> and <paramref name="data"/>
		/// </returns>
		/// <remarks>
		/// The number of bytes written to the span is truncated up to the length of the span. Any
		/// unused bytes within the span will be zero filled up to its length.
		/// </remarks>
		public int Write(ReadOnlySpan<byte> data)
		{
			int length = Math.Min(span.Length, data.Length);

			// Copy up until the end of the span
			data[..length].CopyTo(span);

			// Fill the rest of the span with zeros
			span[length..span.Length].Clear();

			// Return the number of real bytes written
			return length;
		}

		/// <summary>
		/// Writes a string value to a span
		/// </summary>
		/// <param name="value">
		/// The string value to write
		/// </param>
		/// <returns>
		/// The number of characters written to the span
		/// </returns>
		/// <remarks>
		/// The number of characters written to the span is truncated to the number of valid
		/// encoded characters that can fit within the span. Any unused bytes within the span will
		/// be zero filled up to its length.
		/// </remarks>
		public int WriteString(string? value)
		{
			// Encode the string into the span
			_encoder.Convert(value ?? string.Empty, span, true,
				out int chars, out int bytes, out _);

			// Zero fill the rest of the span
			span[bytes..span.Length].Clear();

			// Return the number of chars that were encoded
			return chars;
		}

		/// <summary>
		/// Writes a 16 bit unsigned integer value to a span in little endian format
		/// </summary>
		/// <param name="value">
		/// The value to write
		/// </param>
		public void WriteUInt16Le(ushort value)
			=> BinaryPrimitives.WriteUInt16LittleEndian(span, value);

		/// <summary>
		/// Writes a 32 bit unsigned integer value to a span in little endian format
		/// </summary>
		/// <param name="value">
		/// The value to write
		/// </param>
		public void WriteUInt32Le(uint value)
			=> BinaryPrimitives.WriteUInt32LittleEndian(span, value);

		/// <summary>
		/// Writes a 64 bit unsigned integer value to a span in little endian format
		/// </summary>
		/// <param name="value">
		/// The value to write
		/// </param>
		public void WriteUInt64Le(ulong value)
			=> BinaryPrimitives.WriteUInt64LittleEndian(span, value);

		/// <summary>
		/// Writes a 16 bit unsigned integer value to a span in big endian format
		/// </summary>
		/// <param name="value">
		/// The value to write
		/// </param>
		public void WriteUInt16BigEndian(ushort value)
			=> BinaryPrimitives.WriteUInt16BigEndian(span, value);

		/// <summary>
		/// Writes a 32 bit unsigned integer value to a span in big endian format
		/// </summary>
		/// <param name="value">
		/// The value to write
		/// </param>
		public void WriteUInt32Be(uint value)
			=> BinaryPrimitives.WriteUInt32BigEndian(span, value);

		/// <summary>
		/// Writes a 64 bit unsigned integer value to a span in big endian format
		/// </summary>
		/// <param name="value">
		/// The value to write
		/// </param>
		public void WriteUInt64Be(ulong value)
			=> BinaryPrimitives.WriteUInt64BigEndian(span, value);

		/// <summary>
		/// Writes a 16 bit signed integer value to a span in little endian format
		/// </summary>
		/// <param name="value">
		/// The value to write
		/// </param>
		public void WriteInt16Le(short value)
			=> BinaryPrimitives.WriteInt16LittleEndian(span, value);


		/// <summary>
		/// Writes a 32 bit signed integer value to a span in little endian format
		/// </summary>
		/// <param name="value">
		/// The value to write
		/// </param>
		public void WriteInt32Le(int value)
			=> BinaryPrimitives.WriteInt32LittleEndian(span, value);

		/// <summary>
		/// Writes a 64 bit signed integer value to a span in little endian format
		/// </summary>
		/// <param name="value">
		/// The value to write
		/// </param>
		public void WriteInt64Le(long value)
			=> BinaryPrimitives.WriteInt64LittleEndian(span, value);

		/// <summary>
		/// Writes a 16 bit signed integer value to a span in big endian format
		/// </summary>
		/// <param name="value">
		/// The value to write
		/// </param>
		public void WriteInt16Be(short value)
			=> BinaryPrimitives.WriteInt16BigEndian(span, value);

		/// <summary>
		/// Writes a 32 bit signed integer value to a span in big endian format
		/// </summary>
		/// <param name="value">
		/// The value to write
		/// </param>
		public void WriteInt32Be(int value)
			=> BinaryPrimitives.WriteInt32BigEndian(span, value);

		/// <summary>
		/// Writes a 64 bit signed integer value to a span in big endian format
		/// </summary>
		/// <param name="value">
		/// The value to write
		/// </param>
		public void WriteInt64Be(long value)
			=> BinaryPrimitives.WriteInt64BigEndian(span, value);
	}

	extension(ReadOnlySpan<byte> span)
	{
		/// <summary>
		/// Reads a 16 bit unsigned integer value from a span in little endian format
		/// </summary>
		/// <returns>
		/// The value that was read
		/// </returns>
		public ushort ReadUInt16Le() => BinaryPrimitives.ReadUInt16LittleEndian(span);

		/// <summary>
		/// Reads a 32 bit unsigned integer value from a span in little endian format
		/// </summary>
		/// <returns>
		/// The value that was read
		/// </returns>
		public uint ReadUInt32Le() => BinaryPrimitives.ReadUInt32LittleEndian(span);

		/// <summary>
		/// Reads a 64 bit unsigned integer value from a span in little endian format
		/// </summary>
		/// <returns>
		/// The value that was read
		/// </returns>
		public ulong ReadUInt64Le() => BinaryPrimitives.ReadUInt64LittleEndian(span);

		/// <summary>
		/// Reads a 16 bit unsigned integer value from a span in big endian format
		/// </summary>
		/// <returns>
		/// The value that was read
		/// </returns>
		public ushort ReadUInt16Be() => BinaryPrimitives.ReadUInt16BigEndian(span);

		/// <summary>
		/// Reads a 32 bit unsigned integer value from a span in big endian format
		/// </summary>
		/// <returns>
		/// The value that was read
		/// </returns>
		public uint ReadUInt32Be() => BinaryPrimitives.ReadUInt32BigEndian(span);

		/// <summary>
		/// Reads a 64 bit unsigned integer value from a span in big endian format
		/// </summary>
		/// <returns>
		/// The value that was read
		/// </returns>
		public ulong ReadUInt64Be() => BinaryPrimitives.ReadUInt64BigEndian(span);

		/// <summary>
		/// Reads a 16 bit signed integer value from a span in little endian format
		/// </summary>
		/// <returns>
		/// The value that was read
		/// </returns>
		public short ReadInt16Le() => BinaryPrimitives.ReadInt16LittleEndian(span);

		/// <summary>
		/// Reads a 32 bit signed integer value from a span in little endian format
		/// </summary>
		/// <returns>
		/// The value that was read
		/// </returns>
		public int ReadInt32Le() => BinaryPrimitives.ReadInt32LittleEndian(span);

		/// <summary>
		/// Reads a 64 bit signed integer value from a span in little endian format
		/// </summary>
		/// <returns>
		/// The value that was read
		/// </returns>
		public long ReadInt64Le() => BinaryPrimitives.ReadInt64LittleEndian(span);

		/// <summary>
		/// Reads a 16 bit signed integer value from a span in big endian format
		/// </summary>
		/// <returns>
		/// The value that was read
		/// </returns>
		public short ReadInt16Be() => BinaryPrimitives.ReadInt16BigEndian(span);

		/// <summary>
		/// Reads a 32 bit signed integer value from a span in big endian format
		/// </summary>
		/// <returns>
		/// The value that was read
		/// </returns>
		public int ReadInt32Be() => BinaryPrimitives.ReadInt32BigEndian(span);

		/// <summary>
		/// Reads a 64 bit signed integer value from a span in big endian format
		/// </summary>
		/// <returns>
		/// The value that was read
		/// </returns>
		public long ReadInt64Be() => BinaryPrimitives.ReadInt64BigEndian(span);
	}
}