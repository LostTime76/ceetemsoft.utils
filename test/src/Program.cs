using System.Runtime.CompilerServices;
using CeetemSoft.Utils;

namespace Test;

public static partial class Program
{
	[SkipLocalsInit]
	public static void Main(string[] args)
	{
		byte[] b = new byte[4];

		var s = b.AsSpan();

		int c = b.WriteString("a");
	}
}