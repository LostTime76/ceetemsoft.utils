using CeetemSoft.UcBuild;

namespace Test;

public static class BuildContext
{
	public static TargetDatabase? TargetDatabase => new();

	public static DependsDatabase? DependsDatabase => new();
}