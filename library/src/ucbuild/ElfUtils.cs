using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using CeetemSoft.Utils;

namespace CeetemSoft.UcBuild;

/// <summary>
/// Provides a set of elf file utility functions
/// </summary>
/// <remarks>
/// Since the functions are meant for microcontrollers, only the following features are supported:
/// <list type="number">
/// <item>
/// <description>
/// 32 bit elf files
/// </description>
/// </item>
/// <item>
/// <description>
/// Physical addressing
/// </description>
/// </item>
/// </list>
/// </remarks>
public static class ElfUtils
{
	private const int ET_EXEC   = 2;
	private const int EI_DATA   = 5;
	private const int EI_TYPE   = 0x10;
	private const int EI_PHOFF  = 0x1C;
	private const int EI_PHNUM  = 0x2C;
	private const int EI_END    = 0x34;
	private const int EPI_FOFF  = 0x04;
	private const int EPI_PADDR = 0x0C;
	private const int EPI_FSIZ  = 0x10;
	private const int EPI_END   = 0x20;

	private static readonly byte[] HeaderBaseData = [
		0x7F, 0x45, 0x4C, 0x46, 0x01
	];

	/// <summary>
	/// The default fill value for binary data conversion
	/// </summary>
	public const byte DefaultFill = 0xFF;

	/// <summary>
	/// Converts the executable portion of an elf image into binary data and fills any holes with a
    /// specified value
	/// </summary>
	/// <param name="filepath">
    /// The path to the elf file to read
	/// </param>
	/// <param name="address">
	/// The physical address of the start of the executable image
	/// </param>
	/// <param name="size">
	/// The size of the executable executable image
	/// </param>
    /// <param name="fill">
    /// The byte value to write to holes within the image data
    /// </param>
	/// <returns>
    /// A byte array containing the executable portion of the elf image
    /// </returns>
	/// <exception cref="BadImageFormatException">
    /// Thrown if the data comprising the image is not in an expected format
    /// </exception>
	public static byte[] ToBinary(string filepath, uint address, int size, byte fill = DefaultFill)
	{
		return ToBinary(File.ReadAllBytes(filepath), address, size, fill);
	}

	/// <summary>
	/// Converts the executable portion of an elf image into binary data and fills any holes with a
    /// specified value
	/// </summary>
	/// <param name="data">
	/// The span of data comprising the elf image
	/// </param>
	/// <param name="address">
	/// The physical address of the start of the executable image
	/// </param>
	/// <param name="size">
	/// The size of the executable image
	/// </param>
    /// <param name="fill">
    /// The byte value to write to holes within the image data
    /// </param>
	/// <returns>
    /// A byte array containing the executable portion of the elf image
    /// </returns>
    /// <exception cref="BadImageFormatException">
    /// Thrown if the data comprising the image is not in an expected format
    /// </exception>
	public static byte[] ToBinary(ReadOnlySpan<byte> data,
		uint address, int size, byte fill = DefaultFill)
	{
		// Create the image array
		byte[] output = new byte[size];

		// Fill the image array
		Unsafe.InitBlock(ref output[0], fill, (uint)size);

		// Read the image header
		ByteReader reader = ReadHeader(data, out int e_phoff, out short e_phnum);

		// Read the program segments
		ElfSegment[] segments = ReadSegments(ref reader, e_phoff, e_phnum);

		// Copy the segments to the output
		CopySegments(data, output, segments, address);

		// Return the image data
		return output;
	}

	private static void CopySegments(ReadOnlySpan<byte> input, Span<byte> output,
		ElfSegment[] segments, uint address)
	{
		// Copy all of the segments to the output
		foreach (ElfSegment segment in segments)
		{
			CopySegment(input, output, segment, address);
		}
	}

	private static void CopySegment(ReadOnlySpan<byte> input, Span<byte> output,
		ElfSegment segment, uint address)
	{
		uint end    = (uint)(address + output.Length - 1);
		int  size   = segment.p_filesz;
		uint segEnd = (uint)(segment.p_paddr + size - 1);

		// The segment must be fully contained within the output to be copied
		if ((segment.p_paddr < address) || (segEnd > end))
		{
			return;
		}

		// Calculate output offset
		int offset = (int)(segment.p_paddr - address);

		// Copy the segment
		input.Slice(segment.p_offset, size).CopyTo(output.Slice(offset, size));
	}

	private static ElfSegment[] ReadSegments(ref ByteReader reader, int e_phoff, short e_phnum)
	{
		// Create a list of loadable segments
		List<ElfSegment> segments = [];

		// Read all of the segments within the image
		for (int idx = 0; idx < e_phnum; idx++, e_phoff += EPI_END)
		{
			// Read the next segment
			segments.Add(ReadSegment(ref reader, e_phoff));
		}

		// Return the loadable segments
		return [.. segments];
	}

	private static ElfSegment ReadSegment(ref ByteReader reader, int e_phoff)
	{
		// Read the segment information
		ElfSegment segment = new()
		{
			p_type   = reader.ReadUint32(e_phoff),
			p_offset = reader.ReadInt32(e_phoff + EPI_FOFF),
			p_paddr  = reader.ReadUint32(e_phoff + EPI_PADDR),
			p_filesz = reader.ReadInt32(e_phoff + EPI_FSIZ)
		};

		// Make sure the segment is valid
		if (((ulong)segment.p_offset + (ulong)segment.p_filesz) > (ulong)reader.Data.Length)
		{
			ThrowUnexpectedImage();
		}

		// Return the segment
		return segment;
	}

	private static ByteReader ReadHeader(ReadOnlySpan<byte> data,
		out int e_phoff, out short e_phnum)
	{
		// Make sure we have something to read
		if (data.Length < EI_END)
		{
			ThrowUnexpectedImage();
		}

		// Ensure the base header data is expected
		if (!data[..HeaderBaseData.Length].SequenceEqual(HeaderBaseData))
		{
			ThrowUnexpectedImage();
		}

		// Create a new reader
		ByteReader reader = new(data, data[EI_DATA] == 1);

		// Read the header information
		e_phoff = reader.ReadInt32(EI_PHOFF);
		e_phnum = reader.ReadInt16(EI_PHNUM);

		// Ensure this is an executable file
		if (reader.ReadUint16(EI_TYPE) != ET_EXEC)
		{
			ThrowUnexpectedImage();
		}

		// Make sure there are program headers
		else if (e_phnum <= 0)
		{
			ThrowUnexpectedImage();
		}

		// Ensure the program header table is valid
		if (((ulong)e_phoff + (ulong)(e_phnum * EPI_END)) > (ulong)data.Length)
		{
			ThrowUnexpectedImage();
		}

		// Return the reader
		return reader;
	}

	[DoesNotReturn]
	private static void ThrowUnexpectedImage()
	{
		throw new BadImageFormatException("The elf image is not in a recognized format.");
	}

	private readonly struct ElfSegment
	{
		internal uint p_type   { get; init; }
		internal int  p_offset { get; init; }
		internal uint p_paddr  { get; init; }
		internal int  p_filesz { get; init; }
	}
}