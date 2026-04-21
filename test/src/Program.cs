using System.CommandLine;
using CeetemSoft.Cli;
using CeetemSoft.Utils;

namespace Test;

public static partial class Program
{
	public static void Main(string[] args)
	{
		Console.WriteLine(GitUtils.GetCommit());
	}
}