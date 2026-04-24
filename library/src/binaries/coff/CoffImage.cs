using System.Buffers.Binary;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;

namespace CeetemSoft.Binaries.Coff;

/// <summary>
/// Provides read only access to the information within a coff image
/// </summary>
public sealed class CoffImage
{
	private const short Magic            = 0x108;
	private const int   NameEntrySize    = 8;
	private const int   HeaderEntrySize  = 50;
	private const int   SectionEntrySize = 48;
	private const int   SymbolEntrySize  = 18;

	private readonly byte[] _data;

	private short _targetId;
	private int   _charSize;
	private int   _entry;
	private int   _sectabOffset;
	private int   _symtabOffset;
	private int   _strtabOffset;
	private int   _nsects;
	private int   _nsyms;

	private static readonly HashSet<short> _wordTargets = [0x9D];

	/// <summary>
	/// Creates a new image from data
	/// </summary>
	/// <param name="data">
	/// The data that comprises the image
	/// </param>
	/// <exception cref="ArgumentNullException">
	/// Thrown if <paramref name="data"/> is null
	/// </exception>
	/// <exception cref="BadImageFormatException">
	/// Thrown if the image is not in a valid format or there was a problem loading the image
	/// </exception>
	public CoffImage(byte[] data)
	{
		ArgumentNullException.ThrowIfNull(data, nameof(data));

		_data = data;

		// Load the header
		LoadHeader();

		// Load the sections
		Sections = LoadSections();

		// Load the symbols
		Symbols = LoadSymbols();
	}

	/// <summary>
	/// Converts the image to a flat binary output
	/// </summary>
	/// <param name="address">
	/// The target address of allocated sections that should be included within the binary
	/// </param>
	/// <param name="size">
	/// The size in bytes of the binary
	/// </param>
	/// <param name="fill">
	/// The value to initialize each byte of the binary with
	/// </param>
	/// <param name="swapWords">
	/// For targets with a character size of 2 bytes, indicates if each 2 byte word of the binary
	/// should be swapped, otherwise the parameter is ignored
	/// </param>
	/// <returns>
	/// The converted flat binary
	/// </returns>
	public byte[] ToBytes(int address, int size, byte fill = 0xFF, bool swapWords = false)
	{
		int end = address + size;

		// Create a new binary
		var data = new byte[size];

		// Fill the binary with initial values
		Array.Fill(data, fill);

		// Copy all the section data
		foreach(var section in Sections.Values)
		{
			if (!section.Flags.Allocated)
			{
				continue;
			}

			var paddr = section.PhysicalAddress;
			var vaddr = section.VirtualAddress;
			var src   = GetSectionData(section);
			var dlen  = src.Length;

			if ((paddr >= address) && ((paddr + dlen) <= end))
			{
				src.CopyTo(data.AsSpan(paddr - address));
			}
			else if ((vaddr >= address) && ((vaddr + dlen) <= end))
			{
				src.CopyTo(data.AsSpan(vaddr - address));
			}
		}

		if (swapWords && (CharSize == sizeof(short)))
		{
			for (int count = 0; count < size; count += 2)
			{
				(data[count + 1], data[count]) = (data[count], data[count + 1]);
			}
		}

		return data;
	}

	/// <summary>
	/// Gets the value of a symbol within the image
	/// </summary>
	/// <param name="index">
	/// The index of the symbol
	/// </param>
	/// <returns>
	/// The value of the symbol
	/// </returns>
	public int GetSymbolValue(int index) => Symbols[index].Value;

	/// <summary>
	/// Gets the value of a symbol within the image
	/// </summary>
	/// <param name="name">
	/// The name of the symbol
	/// </param>
	/// <returns>
	/// The value of the symbol
	/// </returns>
	public int GetSymbolValue(string name) => Symbols[name].Value;

	/// <summary>
	/// Gets the data of a section within the image
	/// </summary>
	/// <param name="index">
	/// The index of the section
	/// </param>
	/// <returns>
	/// The section data within the image
	/// </returns>
	public Span<byte> GetSectionData(int index) => GetSectionData(Sections[index]);

	/// <summary>
	/// Gets the data of a section within the image
	/// </summary>
	/// <param name="name">
	/// The name of the section
	/// </param>
	/// <returns>
	/// The section data within the image
	/// </returns>
	public Span<byte> GetSectionData(string name) => GetSectionData(Sections[name]);

	/// <summary>
	/// Gets the data of a section within the image
	/// </summary>
	/// <param name="section">
	/// The section
	/// </param>
	/// <returns>
	/// The section data within the image
	/// </returns>
	public Span<byte> GetSectionData(CoffSection section)
		=> GetData(section.DataOffset, section.DataSize);

	/// <summary>
	/// Creates a new image from data read from a file
	/// </summary>
	/// <param name="filepath">
	/// The path of the file to read
	/// </param>
	/// <returns>
	/// The created image
	/// </returns>
	/// <exception cref="BadImageFormatException">
	/// Thrown if the image is not in a valid format or there was a problem loading the image
	/// </exception>
	public static CoffImage Load(string filepath) => new(File.ReadAllBytes(filepath));

	private void LoadHeader()
	{
		if (_data.Length < HeaderEntrySize)
		{
			ThrowInvalidImage();
		}

		var entry        = GetData(0, HeaderEntrySize);
		var nsects       = BinaryPrimitives.ReadInt16LittleEndian(entry[2..4]);
		var symtabOffset = BinaryPrimitives.ReadInt32LittleEndian(entry[8..12]);
		var nsyms        = BinaryPrimitives.ReadInt32LittleEndian(entry[12..16]);
		var targetId     = BinaryPrimitives.ReadInt16LittleEndian(entry[20..22]);
		var magic        = BinaryPrimitives.ReadInt16LittleEndian(entry[22..24]);
		var tentry       = BinaryPrimitives.ReadInt32LittleEndian(entry[42..46]);
		var sectabOffset = HeaderEntrySize;
		var sectabSize   = nsects * SectionEntrySize;
		var sectabEnd    = sectabOffset + sectabSize - 1;
		var symtabSize   = nsyms * SymbolEntrySize;
		var symtabEnd    = symtabOffset + symtabSize - 1;
		var strtabOffset = symtabEnd + 1;
		var strtabSize   = _data.Length - strtabOffset;

		if (magic != Magic)
		{
			ThrowInvalidMagic(magic);
		}
		else if ((nsects < 0) || (sectabEnd >= _data.Length))
		{
			ThrowInvalidSectionTable();
		}
		else if ((nsyms < 0) || (symtabEnd >= _data.Length))
		{
			ThrowInvalidSymbolTable();
		}
		else if (strtabSize < 0)
		{
			ThrowInvalidStringTable();
		}

		_charSize     = _wordTargets.Contains(targetId) ? 2 : 1;
		_entry        = tentry * _charSize;
		_targetId     = targetId;
		_sectabOffset = sectabOffset;
		_symtabOffset = symtabOffset;
		_strtabOffset = strtabOffset;
		_nsects       = nsects;
		_nsyms        = nsyms;
	}

	private CoffTable<CoffSection> LoadSections()
	{
		var sections = new CoffTable<CoffSection>(_nsects);

		for (int index = 0; index < _nsects; index++)
		{
			var section = LoadSection(index);

			if (!sections.TryAdd(section))
			{
				ThrowDuplicateSection(section.Name);
			}
		}

		return sections;
	}

	private CoffTable<CoffSymbol> LoadSymbols()
	{
		var symbols = new CoffTable<CoffSymbol>();

		for (int index = 0; index < _nsyms; index++)
		{
			var symbol = LoadSymbol(index);

			if (symbol == null)
			{
				continue;
			}
			else if (!symbols.TryAdd(symbol))
			{
				ThrowDuplicateSymbol(symbol.Name);
			}
		}

		return symbols;
	}

	private CoffSection LoadSection(int index)
	{
		var offset     = _sectabOffset + index * SectionEntrySize;
		var entry      = GetData(offset, SectionEntrySize);
		var name       = LoadEntryName(GetData(offset, NameEntrySize));
		var paddr      = BinaryPrimitives.ReadInt32LittleEndian(entry[8..12]);
		var vaddr      = BinaryPrimitives.ReadInt32LittleEndian(entry[12..16]);
		var dataSize   = BinaryPrimitives.ReadInt32LittleEndian(entry[16..20]);
		var dataOffset = BinaryPrimitives.ReadInt32LittleEndian(entry[20..24]);
		var iflags     = BinaryPrimitives.ReadInt32LittleEndian(entry[40..44]);
		var flags      = (CoffSectionFlags)iflags;
		var dataEnd    = dataOffset + dataSize - 1;

		if (flags.Allocated)
		{
			dataSize *= _charSize;
			paddr    *= _charSize;
			vaddr    *= _charSize;
		}
		if (string.IsNullOrEmpty(name))
		{
			ThrowInvalidSectionName(index);
		}
		else if ((dataSize < 0) || (dataEnd >= _data.Length))
		{
			ThrowInvalidSectionData(name);
		}

		return new(name)
		{
			Index           = index,
			PhysicalAddress = paddr,
			VirtualAddress  = vaddr,
			DataOffset      = dataOffset,
			DataSize        = dataSize,
			Flags           = flags
		};
	}

	private CoffSymbol? LoadSymbol(int index)
	{
		var offset  = _symtabOffset + index * SymbolEntrySize;
		var entry   = GetData(offset, SymbolEntrySize);
		var storage = (CoffStorageClass)entry[16];

		if (storage != CoffStorageClass.External)
		{
			return null;
		}

		var name    = LoadEntryName(GetData(offset, NameEntrySize));
		var value   = BinaryPrimitives.ReadInt32LittleEndian(entry[8..12]);
		var section = BinaryPrimitives.ReadInt16LittleEndian(entry[12..14]);

		if (string.IsNullOrEmpty(name))
		{
			ThrowInvalidSymbolName(index);
		}
		else if (section >= Sections.Count)
		{
			ThrowInvalidSymbolSection(name);
		}

		return new(name)
		{
			Index        = index,
			Section      = section,
			Value        = value,
			StorageClass = storage
		};
	}

	private string? LoadEntryName(ReadOnlySpan<byte> entry)
	{
		// If the first 4 bytes are 0, the string is a long string
		if (BinaryPrimitives.ReadInt32LittleEndian(entry[0..4]) == 0)
		{
			// The next 4 bytes are the string table offset
			var offset  = BinaryPrimitives.ReadInt32LittleEndian(entry[4..8]) + _strtabOffset;
			var maxSize = _data.Length - offset;

			// Adjust the entry
			entry = _data.AsSpan(offset, Math.Max(maxSize, 0));
		}

		return LoadString(entry);
	}

	private static string? LoadString(ReadOnlySpan<byte> entry)
	{
		int count;

		// Iterate until we reach a null terminator or the maximum size
		for (count = 0; (count < entry.Length) && (entry[count] != 0); count++);

		// Decode the string
		return count > 0 ? Encoding.ASCII.GetString(entry[0..count]) : null;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	private Span<byte> GetData(int offset, int size) => _data.AsSpan(offset, size);

	[DoesNotReturn]
	private static void ThrowInvalidImage()
	{
		throw new BadImageFormatException();
	}

	[DoesNotReturn]
	private static void ThrowInvalidMagic(int read)
	{
		throw new BadImageFormatException($"Expected magic value '{Magic}' but read '{read}'");
	}

	[DoesNotReturn]
	private static void ThrowInvalidSectionTable()
	{
		throw new BadImageFormatException("Invalid section table");
	}

	[DoesNotReturn]
	private static void ThrowInvalidSymbolTable()
	{
		throw new BadImageFormatException("Invlid symbol table");
	}

	[DoesNotReturn]
	private static void ThrowInvalidStringTable()
	{
		throw new BadImageFormatException("Invalid string table");
	}

	[DoesNotReturn]
	private static void ThrowDuplicateSection(string name)
	{
		throw new BadImageFormatException($"Encountered duplicate section with name '{name}'");
	}

	[DoesNotReturn]
	private static void ThrowDuplicateSymbol(string name)
	{
		throw new BadImageFormatException($"Encountered duplicate symbol with name '{name}'");
	}

	[DoesNotReturn]
	private static void ThrowInvalidSectionName(int index)
	{
		throw new BadImageFormatException($"Invalid name for section at index '{index}'");
	}

	[DoesNotReturn]
	private static void ThrowInvalidSectionData(string name)
	{
		throw new BadImageFormatException($"Invalid data for section '{name}'");
	}

	[DoesNotReturn]
	private static void ThrowInvalidSymbolName(int index)
	{
		throw new BadImageFormatException($"Invalid name for symbol at index '{index}'");
	}

	[DoesNotReturn]
	private static void ThrowInvalidSymbolSection(string name)
	{
		throw new BadImageFormatException($"Invalid section for external symbol '{name}'");
	}

	/// <summary>
	/// Gets the size in bytes of a character within the target architecture
	/// </summary>
	public int CharSize => _charSize;

	/// <summary>
	/// Gets the id of the target that the image was built for
	/// </summary>
	public int TargetId => _targetId;

	/// <summary>
	/// Gets the target starting address of code execution
	/// </summary>
	public int Entry => _entry;

	/// <summary>
	/// Gets the data that comprises the image
	/// </summary>
	public byte[] Data => _data;

	/// <summary>
	/// Gets the sections within the image
	/// </summary>
	public CoffTable<CoffSection> Sections { get; private init; }

	/// <summary>
	/// Gets the external symbols within the image
	/// </summary>
	public CoffTable<CoffSymbol> Symbols { get; private init; }
}