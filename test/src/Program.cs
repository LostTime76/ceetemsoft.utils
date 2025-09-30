using CeetemSoft.Text;

namespace Test;

public static class Program
{
	public static void Main(string[] args)
	{
		Utf8StringBuffer b = new();

		var s = b.Reset(null);
		s = b.Reset("a");
		var ss = b.Alloc("b");
	}
}