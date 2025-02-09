namespace CeetemSoft.Text.Jsonp;

public sealed partial class JsonpStreamReader
{
	public JsonpStreamReader(Stream stream)
	{
		ArgumentNullException.ThrowIfNull(stream, nameof(stream));

		BaseStream = stream;
	}

	public Stream BaseStream { get; private init; }
}