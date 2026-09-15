using CeetemSoft.Io;

namespace Test;

public static partial class Program
{
	public static void Main(string[] args)
	{
		byte[] b = [2,2, 234,234];

		var writer = new ByteWriter(b);

		writer.WriteString(3..4, null);
	}
}