using CeetemSoft.UcBuild;

namespace Test;

public class VersionTarget : CompileTarget
{
	public VersionTarget(string source) : base(source) { }

	protected override bool Prepare()
	{
		return !File.Exists(Source) || base.Prepare();
	}

	protected override bool Execute()
	{
		// (Re)create the file
		File.WriteAllText(Source, null);

		// Use default handling
		return base.Execute();
	}
}