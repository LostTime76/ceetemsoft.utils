using CeetemSoft.Runtime;
using System.Runtime.InteropServices;

namespace CeetemSoft.Native;

/// <summary>
/// Provides a set of native array functions
/// </summary>
unsafe public static class NativeArray
{
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
    /// A pointer to the allocated native array. If <paramref name="span"/> is empty, null
    /// is returned. The memory must be freed using the
    /// <see cref="NativeMemory.Free(void*)"/> function.
    /// </returns>
	public static T* Alloc<T>(ReadOnlySpan<T> span) where T: unmanaged
	{
		if (span.IsEmpty)
		{
			return null;
		}
		
		// Allocate the memory for the array
		int bytes = sizeof(T) * span.Length;
		T*  array = (T*)NativeMemory.Alloc((nuint)bytes);

		// Copy the items within the span to the array
		NativeMemory.Copy(span.AsPointer(), array, (nuint)bytes);

		return array;
	}
}