namespace CeetemSoft.Utils;

/// <summary>
/// Provides a means to read values from a span
/// </summary>
public ref struct SpanReader<T>
{
	/// <summary>
    /// Creates a new span reader
    /// </summary>
    /// <param name="span">
    /// The span to read from
    /// </param>
	public SpanReader(ReadOnlySpan<T> span)
	{
		Span = span;
	}

	/// <summary>
    /// Reads an element from the span without advancing the current reader position
    /// </summary>
    /// <param name="value">
    /// The element read from the span or default
    /// </param>
    /// <returns>
    /// True if an element was read from the span, false if there were no elements left to read
    /// </returns>
	public bool Peek(out T? value)
	{
		if (Left == 0)
		{
			value = default;
			return false;
		}

		value = Span[Index];
		return true;
	}

	/// <summary>
    /// Reads an element from the span without advancing the current reader position
    /// </summary>
    /// <returns>
    /// The element read from the span or default if there were no elements left to read
    /// </returns>
	public T? Peek()
	{
		return Left > 0 ? Span[Index] : default;
	}

	/// <summary>
    /// Reads an element from the span
    /// </summary>
    /// <param name="value">
    /// The element read from the span or default
    /// </param>
    /// <returns>
    /// True if an element was read from the span, false if there were no elements left to read
    /// </returns>
	public bool Read(out T? value)
	{
		if (Left == 0)
		{
			value = default;
			return false;
		}

		value = Span[Index++];
		return true;
	}

	/// <summary>
    /// Reads an element from the span
    /// </summary>
    /// <returns>
    /// The element read from the span or default if there were no elements left to read
    /// </returns>
	public T? Read()
	{
		return Left > 0 ? Span[Index++] : default;
	}

	/// <summary>
    /// Implicitly creates a span reader from an array
    /// </summary>
    /// <param name="array">
    /// The array to read from
    /// </param>
    /// <returns>
    /// A span reader created from the span
    /// </returns>
	public static implicit operator SpanReader<T> (T[]? array)
	{
		return new SpanReader<T>(array);
	}

	/// <summary>
    /// Implicitly creates a span reader from a span
    /// </summary>
    /// <param name="span">
    /// The span to read from
    /// </param>
    /// <returns>
    /// A span reader created from the span
    /// </returns>
	public static implicit operator SpanReader<T> (Span<T> span)
	{
		return new SpanReader<T>(span);
	}

	/// <summary>
    /// Implicitly creates a span reader from a readonly span
    /// </summary>
    /// <param name="span">
    /// The readonly span to read from
    /// </param>
    /// <returns>
    /// A span reader created from the span
    /// </returns>
	public static implicit operator SpanReader<T> (ReadOnlySpan<T> span)
	{
		return new SpanReader<T>(span);
	}

	/// <summary>
    /// Gets the current reader index
    /// </summary>
	public int Index { get; private set; }

	/// <summary>
    /// Gets the number of elements left to read within the span
    /// </summary>
	public int Left => Span.Length - Index;

	/// <summary>
	/// Gets the span that is being read from
	/// </summary>
	public ReadOnlySpan<T> Span { get; private init; }
}