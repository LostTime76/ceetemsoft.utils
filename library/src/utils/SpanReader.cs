using System.Diagnostics.CodeAnalysis;

namespace CeetemSoft.Utils;

/// <summary>
/// Provides a means to read elements within a span
/// </summary>
/// <typeparam name="T">
/// The generic type of the elements within the span to read
/// </typeparam>
public ref struct SpanReader<T>
{
	/// <summary>
	/// Creates a new reader that reads from a given span
	/// </summary>
	/// <param name="span">
	/// The span to read from
	/// </param>
	public SpanReader(ReadOnlySpan<T> span)
	{
		Span  = span;
		Index = 0;
	}

	/// <summary>
    /// Reads the next element within the span
    /// </summary>
    /// <returns>
    /// The next element read from within the span or default if there were no elements left to read
    /// </returns>
	[return: MaybeNull]
	public T Read()
	{
		return Current = Index < Span.Length ? Span[Index++] : default ;
	}

	/// <summary>
	/// Gets the current reader index within the span
	/// </summary>
	public int Index { get; private set; }

	/// <summary>
	/// Gets the number of elements left to read within the span
	/// </summary>
	public int Left => Span.Length - Index;

	/// <summary>
    /// Gets the element that was last read within the span or default if no element was read
    /// </summary>
	[MaybeNull]
	[AllowNull]
	public T Current { get; private set; }

	/// <summary>
	/// Gets the span that is being read from
	/// </summary>
	public ReadOnlySpan<T> Span { get; private set; }
}