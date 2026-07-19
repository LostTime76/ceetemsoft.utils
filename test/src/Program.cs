using CeetemSoft.Io.Ftdi;

namespace Test;

public static partial class Program
{
	public static void Main(string[] args)
	{
		var device = FtdiDevice.GetFirstDevice();

		if (device == null)
		{
			return;
		}
                                                                                                                                                                                                                                                                                                                                                      
		device.Open();
		device.SetFlowControl();
		device.SetBaudRate(3000000);
		device.SetDataCharacteristics();
		device.Write([0xA5, 0x42, 0x55]);


		device.Close();
	}
}