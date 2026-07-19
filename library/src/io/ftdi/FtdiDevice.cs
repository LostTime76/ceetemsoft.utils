using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace CeetemSoft.Io.Ftdi;

/// <summary>
/// Provides a means to access an ftdi device connected to the system that utilizes the
/// d2xx.dll driver
/// </summary>
public partial class FtdiDevice
{
	/// <summary>
	/// Gets the default xon/xoff flow control character
	/// </summary>
	public const byte DefaultXonXoffChar = 0xAA;

	/// <summary>
	/// Gets the default number of data bits for a data transfer
	/// </summary>
	public const byte DefaultDataBits = 8;

	/// <summary>
	/// Gets the default baud rate in bits per second
	/// </summary>
	public const int DefaultBaudRate = 9600;

	/// <summary>
	/// Gets the default number of stop bits for a data transfer
	/// </summary>
	public const FtdiStopBits DefaultStopBits = FtdiStopBits.One;

	/// <summary>
	/// Gets the default parity for a data transfer
	/// </summary>
	public const FtdiParity DefaultParity = FtdiParity.None;

	private readonly int _index;
	private readonly int _flags;

	private long _handle;

	private FtdiDevice(int index, int flags,
		short vendorId, short productId, string product, string serial)
	{
		VendorId  = vendorId;
		ProductId = productId;
		Product   = product;
		Serial    = serial;
		_index    = index;
		_flags    = flags;
	}

	/// <summary>
	/// Opens the device for operation
	/// </summary>
	/// <exception cref="InvalidOperationException">
	/// Thrown if the device is already open
	/// </exception>
	public void Open()
	{
		if (IsOpen)
		{
			ThrowDeviceAlreadyOpen();
		}

		_handle = FtdiNative.OpenDevice(_index);
	}

	/// <summary>
	/// Closes the device for operation
	/// </summary>
	public void Close()
	{
		if (!IsOpen)
		{
			return;
		}

		FtdiNative.CloseDevice(_handle);

		_handle = 0;
	}

	/// <summary>
	/// Sets the flow control mode of the device
	/// </summary>
	/// <param name="flowControl">
	/// The flow control mode to use
	/// </param>
	/// <param name="xon">
	/// The xon character if xon/xoff flow control is used
	/// </param>
	/// <param name="xoff">
	/// The xoff character if xon/xoff flow control is used
	/// </param>
	/// <exception cref="InvalidOperationException">
	/// Thrown if the device is not open
	/// </exception>
	public void SetFlowControl(FtdiFlowControl flowControl = FtdiFlowControl.None,
		byte xon = DefaultXonXoffChar, byte xoff = DefaultXonXoffChar)
	{
		AssertOpen();

		FtdiNative.SetFlowControl(_handle, flowControl, xon, xoff);
	}

	/// <summary>
	/// Sets the characteristics for a data transfer
	/// </summary>
	/// <param name="dataBits">
	/// The number of data bits for the transfer. Only values of 7 or 8 are supported.
	/// </param>
	/// <param name="stopBits">
	/// The number of stop bits
	/// </param>
	/// <param name="parity">
	/// The parity of the data transfer
	/// </param>
	/// <exception cref="InvalidOperationException">
	/// Thrown if the device is not open
	/// </exception>
	public void SetDataCharacteristics(int dataBits = DefaultDataBits,
		FtdiStopBits stopBits = DefaultStopBits, FtdiParity parity = DefaultParity)
	{
		AssertOpen();

		FtdiNative.SetDataCharacteristics(_handle, dataBits, stopBits, parity);
	}

	/// <summary>
	/// Sets the baud rate of the device
	/// </summary>
	/// <param name="baud">
	/// The baud rate in bits per second
	/// </param>
	/// <exception cref="InvalidOperationException">
	/// Thrown if the device is not open
	/// </exception>
	public void SetBaudRate(int baud = DefaultBaudRate)
	{
		AssertOpen();

		FtdiNative.SetBaudRate(_handle, baud);
	}

	/// <summary>
	/// Writes data to the device
	/// </summary>
	/// <param name="data">
	/// The data to write to the device
	/// </param>
	/// <returns>
	/// The number of bytes written to the device
	/// </returns>
	public int Write(ReadOnlySpan<byte> data)
	{
		AssertOpen();

		return FtdiNative.Write(_handle, data);
	}

	/// <summary>
	/// Gets the first device connected to the system that matches a given device id pattern
	/// </summary>
	/// <param name="pattern">
	/// The device id pattern to match against. If the pattern is null,
	/// <see cref="FtdiDeviceIdPattern.Default"/> is used.
	/// </param>
	/// <returns>
	/// The first matching device or null if there are no matches
	/// </returns>
	public static FtdiDevice? GetFirstDevice(FtdiDeviceIdPattern? pattern = null)
	{
		var devices = GetDevices(pattern);

		return devices.Length > 0 ? devices[0] : null;
	}

	/// <summary>
	/// Gets all the devices connected to the system that match a given device id pattern
	/// </summary>
	/// <param name="pattern">
	/// The device id pattern to match against. If the pattern is null,
	/// <see cref="FtdiDeviceIdPattern.Default"/> is used.
	/// </param>
	/// <returns>
	/// An array containing the devices that match the given pattern or an empty array if there
	/// are no matches
	/// </returns>
	public static FtdiDevice[] GetDevices(FtdiDeviceIdPattern? pattern = null)
	{
		pattern ??= FtdiDeviceIdPattern.Default;

		int ndevices = FtdiNative.CreateDeviceInfoList();
		var devices  = new List<FtdiDevice>();

		for (int index = 0; index < ndevices; index++)
		{
			var device = CreateDevice(index, pattern);

			if (device != null)
			{
				devices.Add(device);
			}
		}

		return [..devices];
	}

	private static FtdiDevice? CreateDevice(int index, FtdiDeviceIdPattern pattern)
	{
		FtdiNative.GetDeviceInfo(index, out int flags, out int type, out int usbIds,
			out string product, out string serial);

		var vendorId  = (short)(usbIds >> 16);
		var productId = (short)(usbIds & 0xFFFF);

		if ((flags & FtdiNative.OpenFlag) != 0)
		{
			return null;
		}
		else if (!pattern.IsMatch(vendorId, productId, product, serial))
		{
			return null;
		}

		return new(index, flags, vendorId, productId, product, serial);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	private void AssertOpen()
	{
		if (!IsOpen)
		{
			ThrowDeviceNotOpen();
		}
	}

	[DoesNotReturn]
	private static void ThrowDeviceNotOpen()
	{
		throw new InvalidOperationException("The device must be open for the operation.");
	}

	[DoesNotReturn]
	private static void ThrowDeviceAlreadyOpen()
	{
		throw new InvalidOperationException("The device has already been opened.");
	}

	/// <summary>
	/// Gets a value that indicates if the device is open or not
	/// </summary>
	public bool IsOpen => _handle != 0;

	/// <summary>
	/// Gets a value that indicates if the device is in usb high speed operation
	/// </summary>
	public bool IsHighSpeed => (_flags & FtdiNative.HighSpeedFlag) != 0;

	/// <summary>
	/// Gets the usb vendor id of the device
	/// </summary>
	public short VendorId { get; private init; }

	/// <summary>
	/// Gets the usb product id of the device
	/// </summary>
	public short ProductId { get; private init; }

	/// <summary>
	/// Gets the product description of the device
	/// </summary>
	public string Product { get; private init; }

	/// <summary>
	/// Gets the serial of the device
	/// </summary>
	public string Serial { get; private init; }
}