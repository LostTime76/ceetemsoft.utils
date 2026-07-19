using System.Data.SqlTypes;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace CeetemSoft.Io.Ftdi;

internal static class FtdiNative
{
	internal const int OpenFlag      = 0x01;
	internal const int HighSpeedFlag = 0x02;

	private const string LibraryName = "ftd2xx.dll";

	[DllImport(LibraryName)]
	private extern static FtdiError FT_Open(int index, out long handle);

	[DllImport(LibraryName)]
	private extern static FtdiError FT_Close(long handle);

	[DllImport(LibraryName)]
	private extern static FtdiError FT_SetFlowControl(long handle, int flowControl,
		byte xon, byte xoff);

	[DllImport(LibraryName)]
	private extern static FtdiError FT_SetBaudRate(long handle, int baud);

	[DllImport(LibraryName)]
	private extern static FtdiError FT_CreateDeviceInfoList(out int devices);

	[DllImport(LibraryName)]
	private extern static FtdiError FT_SetDataCharacteristics(long handle, byte dataBits,
		byte stopBits, byte parity);

	[DllImport(LibraryName)]
	private extern static FtdiError FT_Write(long handle,
		in byte data, int bytes, out int written);

	[DllImport(LibraryName)]
	private extern static FtdiError FT_GetDeviceInfoDetail(int index, out int flags, out int type,
		out int usbIds, out int location, in byte serial, in byte product, out long handle);

	internal static int CreateDeviceInfoList()
	{
		AssertError(FT_CreateDeviceInfoList(out int devices));

		return devices;
	}

	internal static void GetDeviceInfo(int index, out int flags, out int type, out int usbIds,
		out string product, out string serial)
	{
		Span<byte> serialBytes  = stackalloc byte[16];
		Span<byte> productBytes = stackalloc byte[64];

		AssertError(FT_GetDeviceInfoDetail(index, out flags, out type, out usbIds, out _,
			in MemoryMarshal.GetReference(serialBytes),
			in MemoryMarshal.GetReference(productBytes), out _));
		
		product = DecodeString(productBytes);
		serial  = DecodeString(serialBytes);
	}

	internal static long OpenDevice(int index)
	{
		AssertError(FT_Open(index, out long handle));

		return handle;
	}

	internal static void CloseDevice(long handle)
	{
		AssertError(FT_Close(handle));
	}

	internal static void SetFlowControl(long handle, FtdiFlowControl flowControl,
		byte xon, byte xoff)
	{
		AssertError(FT_SetFlowControl(handle, (int)flowControl, xon, xoff));
	}

	internal static void SetDataCharacteristics(long handle, int dataBits,
		FtdiStopBits stopBits, FtdiParity parity)
	{
		AssertError(FT_SetDataCharacteristics(handle, (byte)dataBits,
			(byte)stopBits, (byte)parity));
	}

	internal static void SetBaudRate(long handle, int baud)
	{
		AssertError(FT_SetBaudRate(handle, baud));
	}

	internal static int Write(long handle, ReadOnlySpan<byte> data)
	{
		AssertError(FT_Write(handle, MemoryMarshal.GetReference(data),
			data.Length, out int written));

		return written;
	}

	private static string DecodeString(ReadOnlySpan<byte> bytes)
		=> Encoding.UTF8.GetString(bytes[..bytes.IndexOf((byte)0)]);

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	private static void AssertError(FtdiError error, FtdiError expected = FtdiError.Ok)
	{
		if (error != expected)
		{
			ThrowError(error);
		}
	}

	[DoesNotReturn]
	private static void ThrowError(FtdiError error)
	{
		throw new FtdiException(error);
	}
}