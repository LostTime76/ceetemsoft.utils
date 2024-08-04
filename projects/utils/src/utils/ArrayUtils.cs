using System.Runtime.InteropServices;

namespace CeetemSoft.Utils;

/// <summary>
/// Provides a class of array utility functions
/// </summary>
unsafe public static class ArrayUtils
{
	/// <summary>
    /// Creates a managed array of bytes from a native buffer of bytes
    /// </summary>
    /// <param name="src">
    /// A pointer to the native buffer
    /// </param>
    /// <param name="length">
    /// The number of bytes within the native buffer to copy
    /// </param>
    /// <returns>
    /// A managed array of bytes if the operation was successful, otherwise null
    /// </returns>
	public static byte[]? Create(byte* src, int length)
	{
		if ((src == null) || (length < 0))
		{
			return null;
		}

		byte[] bytes = new byte[length];

		NativeMemory.Copy(src, bytes.AsPointer(), (nuint)length);

		return bytes;
	}

    /// <summary>
    /// Allocates an array of bytes and initializes all bytes to a specified fill value
    /// </summary>
    /// <param name="length">
    /// The length of the array in bytes
    /// </param>
    /// <param name="fill">
    /// The value to initialize all bytes within the array to
    /// </param>
    /// <returns>
    /// A newly allocate array of bytes if successful, otherwise null
    /// </returns>
    public static byte[]? Create(int length, byte fill = byte.MaxValue)
    {
        if (length < 0)
        {
			return null;
		}

		byte[] bytes = new byte[length];

		NativeMemory.Fill(bytes.AsPointer(), (nuint)length, fill);

		return bytes;
	}
}