using CeetemSoft.Runtime;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace CeetemSoft.Native;

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
    /// Allocates a native array of pointers to structures. The structures are copied from the
    /// structures within a span. The array is null terminated as the last element is set to null.
    /// </summary>
    /// <typeparam name="T">
    /// The generic unmanaged type
    /// </typeparam>
    /// <param name="span">
    /// The span containing the items to copy into the array
    /// </param>
    /// <returns>
    /// A pointer to the allocated native array. The memory must be freed using the
    /// <see cref="NativeMemory.Free(void*)"/> function.
    /// </returns>
	public static T** Alloc<T>(ReadOnlySpan<T> span) where T: unmanaged
	{
		T* first;

		return Alloc(span, &first);
	}

	/// <summary>
    /// Allocates a native array of pointers to structures. The structures are copied from the
    /// structures within a span. The array is null terminated as the last element is set to null.
    /// </summary>
    /// <typeparam name="T">
    /// The generic unmanaged type
    /// </typeparam>
    /// <param name="span">
    /// The span containing the items to copy into the array
    /// </param>
    /// <param name="first">
    /// A variable to store a pointer to the first item within the array. If the array is empty,
    /// the value is set to null.
    /// </param>
    /// <returns>
    /// A pointer to the allocated native array. The memory must be freed using the
    /// <see cref="NativeMemory.Free(void*)"/> function.
    /// </returns>
	public static T** Alloc<T>(ReadOnlySpan<T> span, T** first) where T: unmanaged
	{
		int length = span.Length;
		int pbytes = sizeof(T*) * (length + 1);
		int dbytes = sizeof(T) * length;
		T** array  = (T**)NativeMemory.Alloc((nuint)(pbytes + dbytes));
		T*  item   = (T*)((long)array + pbytes);

		NativeMemory.Copy(span.AsPointer(), item, (nuint)dbytes);

		for (int idx = 0; idx < length; idx++)
		{
			array[idx] = &item[idx];
		}

		// Null terminate array
		array[length] = null;

		// Set first item
		*first = length > 0 ? item : null;

		return array;
	}
}