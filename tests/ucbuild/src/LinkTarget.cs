using CeetemSoft.UcBuild;
using CeetemSoft.Utils;

namespace Test;

public class LinkTarget : BuildTarget
{
	private static readonly string BaseRule = Sclex.Join([
		"ld.lld.exe", "-o", "{1}", "--script", "{2}", "{0}"
	]);

	protected override void Executed()
	{
		ConsoleUtils.WriteLineIfNotEmpty(Result.Output + Result.Error);
	}

	protected override bool Prepare()
	{
		if (FileUtils.IsOlder(Paths.OutputFilepath,
			FileUtils.GetTimestamp(Paths.LinkerScriptFilepath)))
		{
			BuildContext.VersionTarget.SetOutdated();
			return true;
		}

		return false;
	}

	protected override bool Execute()
	{
		Console.WriteLine("Linking...");

		// Ensure the output directory exists
		DirectoryUtils.TryCreateDirectory(Paths.OutputDirectory);

		// Generate the command line
		string command = string.Format(BaseRule, GetInputs(),
			Paths.OutputFilepath, Paths.LinkerScriptFilepath);

		// Invoke the linker
		return (Result = ProcessUtils.Execute(command)).ExitCode == 0;
	}

	private static string GetInputs()
	{
		return Sclex.Join(BuildContext.Sources.Select(target => target.Object));
	}

	/// <summary>
    /// Gets the result of the linker invocation
    /// </summary>
	public ProcessResult Result { get; private set; }
}