namespace CeetemSoft.Io.Ftdi;

/// <summary>
/// Describes the parity for a data transfer
/// </summary>
public enum FtdiParity
{
	/// <summary>
	/// No parity
	/// </summary>
	None = 0,

	/// <summary>
	/// Odd parity
	/// </summary>
	Odd = 1,

	/// <summary>
	/// Even parity
	/// </summary>
	Even = 2,

	/// <summary>
	/// Mark parity
	/// </summary>
	Mark = 3,

	/// <summary>
	/// Space parity
	/// </summary>
	Space = 4
}