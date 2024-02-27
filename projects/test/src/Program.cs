using CeetemSoft.Utils;

namespace Test;

unsafe public static class Program
{
	public static void Main(string[] args)
	{
		uint a = 0xFFAA5577;

		byte* b = (byte*)&a;

		BitUtils.Rev16(new Span<byte>(b, 4));
	}
}