using CeetemSoft.UcBuild;

namespace Test;

public class BinaryTarget : BuildTarget
{
	private const int Address = 0x8000;
	private const int Size    = 128 * 1024;

	protected override bool Prepare()
	{
		if (!File.Exists(Paths.BinaryFilepath))
		{
			BuildContext.VersionTarget.SetOutdated();
			return true;
		}

		return false;
	}

	protected override bool Execute()
	{
		Console.WriteLine("Generating binary...");

		// Convert the elf file to binary data
		File.WriteAllBytes(Paths.BinaryFilepath,
			ElfUtils.ToBinary(Paths.OutputFilepath, Address, Size));

		// The operation always succeeds
		return true;
	}
}