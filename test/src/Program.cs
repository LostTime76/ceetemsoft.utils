namespace Test;

public static partial class Program
{
	public static void Main(string[] args)
	{
		foreach(string a in args)
		{
			Console.WriteLine(a);
		}
	}
}