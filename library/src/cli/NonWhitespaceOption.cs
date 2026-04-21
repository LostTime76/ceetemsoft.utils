using System.CommandLine;
using System.CommandLine.Parsing;

namespace CeetemSoft.Cli;

/// <summary>
/// Provides a basic string argument that publishes an error if it contains only whitespace
/// </summary>
public class NonWhitespaceOption : Option <string>
{
	/// <summary>
	/// Creates a new argument
	/// </summary>
	/// <param name="name">
	/// The name of the argument
	/// </param>
	public NonWhitespaceOption(string name) : base(name)
	{
		Validators.Add(Validate);
	}

	private static void Validate(OptionResult result)
	{
		if (string.IsNullOrWhiteSpace(result.Tokens[0].Value))
		{
			result.AddError($"'{result.Option.Name}' cannot only contain whitespace characters.");
		}
	}
}
