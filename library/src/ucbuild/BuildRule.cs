namespace CeetemSoft.UcBuild;

public sealed class BuildRule
{
	public BuildRule(string executable, string? argumentFormat)
	{
		ArgumentException.ThrowIfNullOrEmpty(executable, nameof(executable));

		Executable     = executable;
		Format         = GetFormat(executable, argumentFormat);
		ArgumentFormat = argumentFormat;
	}

	private static string GetFormat(string executable, string? argumentFormat)
	{
		return argumentFormat != null ? $"{executable} {argumentFormat}" : executable;
	}

	public override string ToString() => Format;

	public string Executable { get; private init; }

	public string Format { get; private init; }

	public string? ArgumentFormat { get; private init; }
}