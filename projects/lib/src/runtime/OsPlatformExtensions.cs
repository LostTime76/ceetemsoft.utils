using System.Runtime.InteropServices;

namespace CeetemSoft.Runtime;

/// <summary>
/// Provides a set of operating system platform extension members
/// </summary>
public static class OsPlatformExtensions
{
	/// <summary>
	/// Gets the current operating system
	/// </summary>
	/// <returns>
	/// The current operating system
	/// </returns>
	/// <exception cref="PlatformNotSupportedException">
	/// Thrown if the current operating system is not supported
	/// </exception>
	public static OSPlatform GetPlatform()
	{
		if (OperatingSystem.IsWindows())
		{
			return OSPlatform.Windows;
		}
		else if (OperatingSystem.IsLinux())
		{
			return OSPlatform.Linux;
		}
		else if (OperatingSystem.IsMacOS())
		{
			return OSPlatform.OSX;
		}

		throw new PlatformNotSupportedException();
	}
}