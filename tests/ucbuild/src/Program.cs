using CeetemSoft.UcBuild;

namespace Test;

unsafe public static class Program
{
	public static void Main()
	{
		// Create a new build
		MicroBuild build = new()
		{
			MaxThreads = Environment.ProcessorCount - 2,
			Executing  = BuildExecuting
		};

		// Create target dependencies
		BuildContext.VersionTarget.Dependencies.AddRange(BuildContext.Sources);
		BuildContext.LinkTarget.Dependencies.Add(BuildContext.VersionTarget);
		BuildContext.BinaryTarget.Dependencies.Add(BuildContext.LinkTarget);

		// Execute the build
		BuildResult result = build.Execute(BuildContext.BinaryTarget);

		// Save the build context
		BuildContext.Save();
	}

	private static void BuildExecuting()
	{
		// Get the number of regular sources needed to be built
		int sources = BuildContext.Sources.Count(target => target.IsOutdated);

		// Add version target
		sources += BuildContext.VersionTarget.IsOutdated ? 1 : 0;

		// Set the number of sources that need to be built: include version target
		BuildContext.SourcesToBuild = sources;
	}
}