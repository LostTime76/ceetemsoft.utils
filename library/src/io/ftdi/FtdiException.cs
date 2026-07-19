namespace CeetemSoft.Io.Ftdi;

/// <summary>
/// Provides an exception for ftdi device operations
/// </summary>
public class FtdiException : Exception
{
	/// <summary>
	/// Creates a new exception with the given error
	/// </summary>
	/// <param name="error">
	/// The error associated with the exception
	/// </param>
	public FtdiException(FtdiError error) : this(null, error, null) { }

	/// <summary>
	/// Creates a new exception with the given message and error
	/// </summary>
	/// <param name="message">
	/// The exception message
	/// </param>
	/// <param name="error">
	/// The error associated with the exception
	/// </param>
	public FtdiException(string? message, FtdiError error) : this(message, error, null) { }

	/// <summary>
	/// Creates a new exception with the given message, error, and inner exception
	/// </summary>
	/// <param name="message">
	/// The exception message
	/// </param>
	/// <param name="error">
	/// The error associated with the exception
	/// </param>
	/// <param name="inner">
	/// The inner exception
	/// </param>
	public FtdiException(string? message, FtdiError error, Exception? inner) : base(message, inner)
	{
		Error = error;
	}

	/// <summary>
	/// Gets the error associated with the exception
	/// </summary>
	public FtdiError Error { get; private init; }
}