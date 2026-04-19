namespace CeetemSoft.Binaries.Coff;

/// <summary>
/// Provides read only access to the information about a symbol within a coff image
/// </summary>
public sealed class CoffSymbol : CoffEntry
{
	internal CoffSymbol(string name) : base(name) { }

	/// <summary>
	/// Gets the index of the symbol as it appears within the coff image
	/// </summary>
	public int Index { get; internal init; }

	/// <summary>
	/// Gets the index of the section that the symbol resides in
	/// </summary>
	public int Section { get; internal init; }

	/// <summary>
	/// Gets the value of the symbol
	/// </summary>
	public int Value { get; internal init; }

	/// <summary>
	/// Gets the storage class of the symbol
	/// </summary>
	public CoffStorageClass StorageClass { get; internal init; }
}