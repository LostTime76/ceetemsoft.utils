namespace CeetemSoft.UcBuild;

public static class BuildGroup
{
	public const int DefaultMaxThreads = 1;

	public static BuildTarget[] GetOutdatedTargets(
		IEnumerable<BuildTarget> targets, int maxThreads = DefaultMaxThreads)
	{
		return default;
	}

	public static bool Execute(
		IEnumerable<BuildTarget> targets, int maxThreads = DefaultMaxThreads)
	{
		return default;
	}
}