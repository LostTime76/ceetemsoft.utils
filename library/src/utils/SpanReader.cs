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
    /// Reads the next element within the span
    /// </summary>
    /// <returns>
    /// The next element read from within the span or default if there are no more elements to read
    /// </returns>
	[return: MaybeNull]
	public T Read()
	{
		return Current = Index < Span.Length ? Span[Index++] : default;
	}

	/// <summary>
	/// Reads all the elements that are left to be read from the span
	/// </summary>
	/// <returns>
	/// Null if no elements were read, otherwise an enumerable containing the elements read from
	/// the span
	/// </returns>
	public IEnumerable<T>? ReadLeft()
	{
		return Read(Left);
	}

	/// <summary>
	/// Reads a number of elements from the span
	/// </summary>
	/// <param name="elements">
	/// The number of elements to read
	/// </param>
	/// <returns>
	/// Null if no elements were read, otherwise an enumerable containing the elements read from
	/// the span
	/// </returns>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Thrown if <paramref name="elements"/> is less than 0 or greater than <see cref="Left"/>
	/// </exception>
	public IEnumerable<T>? Read(int elements)
	{
		if (elements == 0)
		{
			return null;
		}
		else if ((elements < 0) || (elements > Left))
		{
			ThrowInvalidNumberOfElements(nameof(elements));
		}

		List<T> outputs = [];

		for(int count = 0; count < elements; count++)
		{
			outputs.Add(Read()!);
		}

		return outputs;
	}

	/// <summary>
    /// Implicitly converts a span to a reader
    /// </summary>
    /// <param name="span">
    /// The span to convert to a reader
    /// </param>
	public static implicit operator SpanReader<T>(Span<T> span)
	{
		return new(span);
	}

	/// <summary>
    /// Implicitly converts a span to a reader
    /// </summary>
    /// <param name="span">
    /// The span to convert to a reader
    /// </param>
	public static implicit operator SpanReader<T>(ReadOnlySpan<T> span)
	{
		return new(span);
	}

	/// <summary>
    /// Implicitly converts an array to a reader
    /// </summary>
    /// <param name="array">
    /// The array to convert to a reader
    /// </param>
	public static implicit operator SpanReader<T>(T[] array)
	{
		return new(array.AsSpan());
	}

	private static void ThrowInvalidNumberOfElements(string param)
	{
		throw new ArgumentOutOfRangeException(param);
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