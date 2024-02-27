using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace CeetemSoft.Utils;

/// <summary>
/// Provides a class of span extension functions
/// </summary>
unsafe public static class SpanExtensions
{
	/// <summary>
	/// Gets a pointer to the first element within a readonly span
	/// </summary>
	/// <typeparam name="T">
	/// The generic blittable type of the span
	/// </typeparam>
	/// <param name="span">
	/// The span to get the pointer from
	/// </param>
	/// <returns>
	/// A pointer to the first element within the span
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static T* AsPointer<T>(this ReadOnlySpan<T> span) where T: unmanaged
	{
		return (T*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(span));
	}

	/// <summary>
	/// Gets a pointer to the first element within a span
	/// </summary>
	/// <typeparam name="T">
	/// The generic blittable type of the span
	/// </typeparam>
	/// <param name="span">
	/// The span to get the pointer from
	/// </param>
	/// <returns>
	/// A pointer to the first element within the span
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static T* AsPointer<T>(this Span<T> span) where T: unmanaged
	{
		return (T*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(span));
	}

	/// <summary>
    /// Gets a pointer to the first element within an array
    /// </summary>
    /// <typeparam name="T">
    /// The generic blittable type of the array
    /// </typeparam>
    /// <param name="array">
    /// The array to get the pointer from
    /// </param>
    /// <returns>
    /// A pointer to the first element within the array if successful, otherwise null
    /// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static T* AsPointer<T>(this T[]? array) where T: unmanaged
	{
		return array != null ? (T*)Unsafe.AsPointer(ref array[0]) : null;
	}
}