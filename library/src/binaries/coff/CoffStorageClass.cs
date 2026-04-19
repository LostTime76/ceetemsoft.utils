namespace CeetemSoft.Binaries.Coff;

/// <summary>
/// Represents the storage class of a symbol within a coff image
/// </summary>
public enum CoffStorageClass
{
	/// <summary>
	/// No storage class
	/// </summary>
	Null = 0,

	/// <summary>
	/// External (global) symbol
	/// </summary>
	External = 2
}