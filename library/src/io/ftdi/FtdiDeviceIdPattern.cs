using System.Text.RegularExpressions;

namespace CeetemSoft.Io.Ftdi;

/// <summary>
/// Provides a means to match the usb properties of an ftdi device against a set of patterns
/// </summary>
public sealed partial class FtdiDeviceIdPattern
{
	private const string UsbIdStringFormat = "x4";

	/// <summary>
	/// Gets the default pattern which matches any input
	/// </summary>
	public static readonly Regex MatchAllPattern = GetMatchAllPattern();

	/// <summary>
	/// Gets the default device id pattern which matches any usb device properties
	/// </summary>
	public static readonly FtdiDeviceIdPattern Default = new();

	/// <summary>
	/// Creates a new pattern
	/// </summary>
	/// <param name="vendorIdPattern">
	/// The vendor id pattern to match against. If the pattern is null,
	/// <see cref="MatchAllPattern"/> is used.
	/// </param>
	/// <param name="productIdPattern"></param>
	/// The product id pattern to match against. If the pattern is null,
	/// <see cref="MatchAllPattern"/> is used.
	/// <param name="productPattern"></param>
	/// The product description pattern to match against. If the pattern is null,
	/// <see cref="MatchAllPattern"/> is used.
	/// <param name="serialPattern"></param>
	/// The serial pattern to match against. If the pattern is null,
	/// <see cref="MatchAllPattern"/> is used.
	public FtdiDeviceIdPattern(Regex? vendorIdPattern = null, Regex? productIdPattern = null,
		Regex? productPattern = null, Regex? serialPattern = null)
	{
		VendorIdPattern  = vendorIdPattern ?? MatchAllPattern;
		ProductIdPattern = productIdPattern ?? MatchAllPattern;
		ProductPattern   = productPattern ?? MatchAllPattern;
		SerialPattern    = serialPattern ?? MatchAllPattern;
	}

	/// <summary>
	/// Determines if the given usb properties match a given set of patterns
	/// </summary>
	/// <param name="vendorId">
	/// The vendor id of the device
	/// </param>
	/// <param name="productId">
	/// The product id of the device
	/// </param>
	/// <param name="product">
	/// The product description of the device. If the string is null, or contains only whitespace,
	/// the function will return false.
	/// </param>
	/// <param name="serial">
	/// the serial of the device. If the string is null, or contains only whitespace, the function
	/// will return false.
	/// </param>
	/// <returns>
	/// True if all the properties match the given patterns, otherwise false
	/// </returns>
	public bool IsMatch(short vendorId, short productId, string product, string serial)
	{
		if (string.IsNullOrWhiteSpace(product) || string.IsNullOrWhiteSpace(serial))
		{
			return false;
		}
		else if (!VendorIdPattern.IsMatch(vendorId.ToString(UsbIdStringFormat)))
		{
			return false;
		}
		else if (!ProductIdPattern.IsMatch(productId.ToString(UsbIdStringFormat)))
		{
			return false;
		}
		else if (!ProductPattern.IsMatch(product))
		{
			return false;
		}

		return SerialPattern.IsMatch(serial);
	}

	[GeneratedRegex(".*")]
	private static partial Regex GetMatchAllPattern();

	/// <summary>
	/// Gets the pattern that the usb vendor id is matched against
	/// </summary>
	public Regex VendorIdPattern { get; private init; }

	/// <summary>
	/// Gets the pattern that the usb product id is matched against
	/// </summary>
	public Regex ProductIdPattern { get; private init; }

	/// <summary>
	/// Gets the pattern that the usb product description is matched against
	/// </summary>
	public Regex ProductPattern { get; private init; }

	/// <summary>
	/// Gets the pattern that the usb serial is matched against
	/// </summary>
	public Regex SerialPattern { get; private init; }
}