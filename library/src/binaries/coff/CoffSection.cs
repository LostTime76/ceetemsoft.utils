namespace CeetemSoft.Binaries.Coff;

/// <summary>
/// Provides read only access to the information about a section within a coff image
/// </summary>
public sealed class CoffSection : CoffEntry
{
	internal CoffSection(string name) : base(name) { }

	/// <summary>
	/// Gets the index of the section as it appears within the coff image
	/// </summary>
	public int Index { get; internal init; }

	/// <summary>
	/// Gets the offset of the section data within the coff image
	/// </summary>
	public int DataOffset { get; internal init; }

	/// <summary>
	/// Gets the size in bytes of the section data within the coff image
	/// </summary>
	public int DataSize { get; internal init; }

	/// <summary>
	/// Gets the physical address of the section within target memory
	/// </summary>
	/// <remarks>
	/// This property has no meaning unless the section is allocated.
	/// </remarks>
	public int PhysicalAddress { get; internal init; }

	/// <summary>
	/// Gets the virtual address of the section within target memory
	/// </summary>
	/// <remarks>
	/// This property has no meaning unless the section is allocated
	/// </remarks>
	public int VirtualAddress { get; internal init; }

	/// <summary>
	/// Gets the flags detailing information about the section
	/// </summary>
	public CoffSectionFlags Flags { get; internal init; }
}