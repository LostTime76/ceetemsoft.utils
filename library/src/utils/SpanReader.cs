using System.Diagnostics.CodeAnalysis;

namespace CeetemSoft.Utils;

/// <summary>
/// Provides a means to read elements from within a span
/// </summary>
/// <typeparam name="T">
/// The generic type of the span that is being read from
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
	/// Reads an element from the span
	/// </summary>
	/// <returns>
	/// The element read from the span
	/// </returns>
	/// <exception cref="InvalidOperationException">
	/// Thrown if there are no more elements left to read from the span
	/// </exception>
	public T Read()
	{
		if (Index == Span.Length)
		{
			ThrowExhausted();
		}

		return Current = Span[Index++];
	}

	private static void ThrowExhausted() =>
		throw new InvalidOperationException("There are no more elements to read.");

	/// <summary>
    /// Implicitly converts a span to a reader
    /// </summary>
    /// <param name="span">
    /// The span to convert to a reader
    /// </param>
	public static implicit operator SpanReader<T>(ReadOnlySpan<T> span) => new(span);

	/// <summary>
    /// Implicitly converts an array to a reader
    /// </summary>
    /// <param name="array">
    /// The array to convert to a reader
    /// </param>
	public static implicit operator SpanReader<T>(T[] array) =>	new(array.AsSpan());

	/// <summary>
	/// Gets the current reader index
	/// </summary>
	public int Index { get; private set; }

	/// <summary>
	/// Gets the number of elements left to read from the span
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