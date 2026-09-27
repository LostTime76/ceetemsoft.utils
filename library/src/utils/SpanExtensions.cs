using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace CeetemSoft.Utils;

/// <summary>
/// Provides a set of span extension members
/// </summary>
public static class SpanExtensions
{
	extension(Span<byte> span)
	{
		/// <summary>
		/// Writes a string value to a span
		/// </summary>
		/// <param name="value">
		/// The string value to write
		/// </param>
		/// <param name="encoding">
		/// The encoding used to encode the string into byte data or null to use utf8
		/// </param>
		public void WriteString(string? value, Encoding? encoding = null)
		{
			// Default encoding is utf8
			encoding ??= Encoding.UTF8;

			// Encode the string into bytes
			var bytes  = value != null ? encoding.GetBytes(value) : [];
			int length = Math.Min(span.Length, bytes.Length);

			// Copy up until the end of the span
			bytes[..length].CopyTo(span);

			// Fill the rest of the span with zeros
			span[length..span.Length].Clear();
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