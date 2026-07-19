namespace CeetemSoft.Io.Ftdi;

/// <summary>
/// Describes the flow control settings for an ftdi device
/// </summary>
public enum FtdiFlowControl
{
	/// <summary>
	/// No flow control
	/// </summary>
	None = 0,

	/// <summary>
	/// Standard rts / cts flow control
	/// </summary>
	RtsCts = 0x100,

	/// <summary>
	/// Dtr / dsr flow control
	/// </summary>
	DtrDsr = 0x0200,

	/// <summary>
	/// Standard xon / xoff flow control
	/// </summary>
	XonXoff = 0x0400
}