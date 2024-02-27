using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace CeetemSoft.Utils;

/// <summary>
/// Provides a class of buffer utility functions
/// </summary>
unsafe public static class BufferUtils
{
	/// <summary>
    /// Allocates a native buffer of bytes filled with the contents of a managed array
    /// </summary>
    /// <param name="bytes">
    /// The managed array with the contents to copy
    /// </param>
    /// <returns>
    /// A native buffer of bytes if the operation is successful, otherwise null. The memory
    /// allocated within this function must manually be freed using the
    /// <see cref="NativeMemory.Free(void*)"/> function.
    /// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static byte* Alloc(byte[] bytes)
	{
		return bytes != null ? Alloc(bytes.AsSpan()) : null;
	}

	/// <summary>
    /// Allocates a native buffer of bytes filled with the contents of a span of bytes
    /// </summary>
    /// <param name="bytes">
    /// The span of bytes with the contents to copy
    /// </param>
    /// <returns>
    /// A native buffer of bytes. The memory allocated within this function must manually be freed
    /// using the <see cref="NativeMemory.Free(void*)"/> function.
    /// </returns>
	public static byte* Alloc(ReadOnlySpan<byte> bytes)
	{
		byte* dst = (byte*)NativeMemory.Alloc((nuint)bytes.Length);
		NativeMemory.Copy(bytes.AsPointer(), dst, (nuint)bytes.Length);
		return dst;
	}
}