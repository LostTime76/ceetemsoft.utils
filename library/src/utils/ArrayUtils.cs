namespace CeetemSoft.Utils;

/// <summary>
/// Provides a set of utility functions for arrays
/// </summary>
public static class ArrayUtils
{
	/// <summary>
    /// Inserts the items within a span into the array at a specified index and returns a new
    /// array. The input array is not modified.
    /// </summary>
    /// <typeparam name="T">
    /// The generic type of the items within the array
    /// </typeparam>
    /// <param name="array">
    /// The array to insert the items into
    /// </param>
    /// <param name="items">
    /// The span containing the items to insert
    /// </param>
    /// <param name="index">
    /// The index at which to insert the items into the array
    /// </param>
    /// <returns>
    /// A new array is returned that contains both the items from the original array and those that
    /// were inserted from the span.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="array"/> is null
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if <paramref name="index"/> is outside the insertion range of
    /// <paramref name="array"/>
    /// </exception>
	public static T[] Insert<T>(T[] array, ReadOnlySpan<T> items, int index = 0)
	{
		if (items.Length == 0)
		{
			return [.. array];
		}

		// Create the array to insert into
		T[] resized = CreateInsertionArray(array, items.Length, index);

		// Insert the items into the array
		items.CopyTo(resized.AsSpan(index, items.Length));

		// Return the resized array
		return resized;
	}

	/// <summary>
    /// Inserts the items within another array into the array at a specified index and returns a new
    /// array. The input array is not modified.
    /// </summary>
    /// <typeparam name="T">
    /// The generic type of the items within the array
    /// </typeparam>
    /// <param name="array">
    /// The array to insert the items into
    /// </param>
    /// <param name="items">
    /// The other array containing the items to insert
    /// </param>
    /// <param name="index">
    /// The index at which to insert the items into the array
    /// </param>
    /// <returns>
    /// A new array is returned that contains both the items from the original array and those that
    /// were inserted from the other array.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="array"/> is null
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if <paramref name="index"/> is outside the insertion range of
    /// <paramref name="array"/>
    /// </exception>
	public static T[] Insert<T>(T[] array, T[]? items, int index = 0)
	{
		return Insert(array, items.AsSpan(), index);
	}

	/// <summary>
    /// Inserts the items within a enumerable into the array at a specified index and returns a new
    /// array. The input array is not modified.
    /// </summary>
    /// <typeparam name="T">
    /// The generic type of the items within the array
    /// </typeparam>
    /// <param name="array">
    /// The array to insert the items into
    /// </param>
    /// <param name="items">
    /// The enumerable containing the items to insert
    /// </param>
    /// <param name="index">
    /// The index at which to insert the items into the array
    /// </param>
    /// <returns>
    /// A new array is returned that contains both the items from the original array and those that
    /// were inserted from the enumerable.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="array"/> is null
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if <paramref name="index"/> is outside the insertion range of
    /// <paramref name="array"/>
    /// </exception>
	public static T[] Insert<T>(T[] array, IEnumerable<T>? items, int index = 0)
	{
		if (items == null)
		{
			return [.. array];
		}

		// Create the array to insert into
		T[] resized = CreateInsertionArray(array, items.Count(), index);

		// Insert the items into the new array
		foreach (T item in items)
		{
			resized[index++] = item;
		}

		// Return the resized array
		return resized;
	}

	private static T[] CreateInsertionArray<T>(T[] array, int items, int index)
	{
		ArgumentNullException.ThrowIfNull(array, nameof(array));

		// Create the resized array
		int alen    = array.Length;
		T[] resized = new T[alen + items];

		// Make sure the insertion index is valid
		if ((index < 0) || (index > alen))
		{
			ThrowIndexOutOfRange(nameof(index));
		}

		// Copy the original items up to the index
		Array.Copy(array, resized, index);

		// Copy the original items after the index
		Array.Copy(array, index, resized, index + items, alen - index);

		// Return the resized array
		return resized;
	}

	private static void ThrowIndexOutOfRange(string param)
	{
		throw new ArgumentOutOfRangeException(param);
	}
}