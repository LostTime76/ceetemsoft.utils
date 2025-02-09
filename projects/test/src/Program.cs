using CeetemSoft.Text.Jsonp;

namespace Test;

unsafe public static class Program
{
	public static void Main()
	{
		// Create a jsonp reader
		JsonpStreamReader reader = new(
			new FileStream("test.jsonp", FileMode.Open, FileAccess.Read));

		
	}
}