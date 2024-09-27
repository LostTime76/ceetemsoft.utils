namespace CeetemSoft.Utils;

/// <summary>
/// Provides flags for controlling the operations of the Sclex class
/// </summary>
[Flags]
public enum SclexOptions
{
	/// <summary>
    /// The operation should do the minimum
    /// </summary>
	None,

	/// <summary>
    /// Escape any input by surrounding it in double quotes if it contains whitespace.
    /// Additionally, any quote character is escaped with a backslash.
    /// </summary>
	Escape = 0x01,

	/// <summary>
    /// Trim whitespace from the beginning and end of any input
    /// </summary>
	Trim = 0x02,

	/// <summary>
    /// The default options for Sclex operations
    /// </summary>
	Default = Escape | Trim
}