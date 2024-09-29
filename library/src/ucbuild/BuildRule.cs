using CeetemSoft.Utils;

namespace CeetemSoft.UcBuild;

public sealed class BuildRule
{
	public string Executable { get; internal init; }

	public string PrimaryOutputExt { get; internal init; }

	public string ArgumentFormat { get; internal init; }

	public string? ResponseFormat { get; internal init; }

	public string? ResponseExt { get; internal init; }

	public string? DependsExt { get; internal init; }

	public string[] OutputExts { get; internal init; }
}