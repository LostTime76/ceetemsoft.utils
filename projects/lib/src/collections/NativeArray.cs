using CeetemSoft.Runtime;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace CeetemSoft.Collections;

/// <summary>
/// Provides a set of native array functions
/// </summary>
unsafe public static class NativeArray
{
	/// <summary>
    /// Gets the number of items within a null terminated native array
    /// </summary>
    /// <typeparam name="T">
    /// The generic type of the items within the array
    /// </typeparam>
    /// <param name="array">
    /// The null terminated native array to get the length of
    /// </param>
    /// <returns>
    /// The number of items within the array if the array is not null, otherwise null
    /// </returns>
	public static int GetLength<T>(T** array) where T: unmanaged
	{
		if (array == null)
		{
			return -1;
		}

		int length = 0;

		for (; array[length] != null; length++);

		return length;
	}

	/// <summary>
    /// Allocates a null terminated native array that contains the items within a span
    /// </summary>
    /// <typeparam name="T">
    /// The generic type of the items within the array
    /// </typeparam>
    /// <param name="span">
    /// The span containing the items to copy
    /// </param>
    /// <returns>
    /// A pointer to the allocated null terminated native array. The memory allocated by this
    /// function must be freed using the <see cref="NativeMemory.Free(void*)"/> function.
    /// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static T** Alloc<T>(ReadOnlySpan<T> span) where T: unmanaged
	{
		T* first;

		return Alloc(span, &first);
	}

	/// <summary>
    /// Allocates a null terminated native array that contains the items within a managed span
    /// </summary>
    /// <typeparam name="T">
    /// The generic type of the items within the array
    /// </typeparam>
    /// <param name="span">
    /// The span containing the items to copy
    /// </param>
    /// <param name="first">
    /// A variable to store a pointer to the first item within the array. If the array is empty,
    /// this value is set to null.
    /// </param>
    /// <returns>
    /// A pointer to the allocated null terminated native array. The memory allocated by this
    /// function must be freed using the <see cref="NativeMemory.Free(void*)"/> function.
    /// </returns>
	public static T** Alloc<T>(ReadOnlySpan<T> span, T** first) where T: unmanaged
	{
		int length = span.Length;
		int pbytes = sizeof(T*) * (length + 1);
		int dbytes = sizeof(T) * length;
		T** array  = (T**)NativeMemory.Alloc((nuint)(pbytes + dbytes));
		T*  item   = (T*)((long)array + pbytes);

		// Copy structures
		NativeMemory.Copy(span.AsPointer(), first, (nuint)dbytes);

		// Copy pointers
		for (int idx = 0; idx < length; idx++)
		{
			array[idx] = &item[idx];
		}

		// Set the pointer to the first item within the array
		*first = length > 0 ? item : null;

		// Null terminate the array
		array[length] = null;

		return array;
	}
}