namespace CeetemSoft.Collections;

/// <summary>
/// Provides a set of array extension members
/// </summary>
public static class ArrayExtensions
{
	/// <summary>
	/// Inserts a span of items into an array
	/// </summary>
	/// <typeparam name="T">
	/// The generic type of the array
	/// </typeparam>
	/// <param name="array">
	/// The array to insert into
	/// </param>
	/// <param name="index">
	/// The insertion index within the array
	/// </param>
	/// <param name="items">
	/// The span of items to insert into the array
	/// </param>
	/// <returns>
	/// A new array containing the inserted items
	/// </returns>
	/// <exception cref="ArgumentNullException">
	/// Thrown if <paramref name="array"/> is null
	/// </exception>
	/// <exception cref="IndexOutOfRangeException">
	/// Thrown if <paramref name="index"/> is outside the range of the array
	/// </exception>
	public static T[] InsertRange<T>(this T[] array, int index, ReadOnlySpan<T> items)
	{
		return items.IsEmpty ? [.. array] : InsertRange(array, index, items, null);
	}

	/// <summary>
	/// Inserts an enumerable of items into an array
	/// </summary>
	/// <typeparam name="T">
	/// The generic type of the array
	/// </typeparam>
	/// <param name="array">
	/// The array to insert into
	/// </param>
	/// <param name="index">
	/// The insertion index within the array
	/// </param>
	/// <param name="items">
	/// The enumerable of items to insert into the array
	/// </param>
	/// <returns>
	/// A new array containing the inserted items
	/// </returns>
	/// <exception cref="ArgumentNullException">
	/// Thrown if <paramref name="array"/> is null
	/// </exception>
	/// <exception cref="IndexOutOfRangeException">
	/// Thrown if <paramref name="index"/> is outside the range of the array
	/// </exception>
	public static T[] InsertRange<T>(this T[] array, int index, IEnumerable<T>? items)
	{
		return items != null ? InsertRange(array, index, default, items) : [.. array];
	}

	private static T[] InsertRange<T>(
		this T[] array, int index, ReadOnlySpan<T> sitems, IEnumerable<T>? eitems)
	{
		ArgumentNullException.ThrowIfNull(array, nameof(array));
		AssertValidIndex(index, array.Length);

		// Create a new list with the original items
		List<T> list = new(array);

		// If the enumerable is null, the span is valid
		if (eitems != null)
		{
			list.InsertRange(index, eitems);
		}
		else
		{
			list.InsertRange(index, sitems);
		}

		// Create the array
		return [.. list];
	}

	private static void AssertValidIndex(int index, int alength)
	{
		if ((index < 0) || (index > alength))
		{
			ThrowIndexOutOfRange();
		}
	}

	private static void ThrowIndexOutOfRange()
	{
		throw new IndexOutOfRangeException();
	}
}