using CeetemSoft.UcBuild;

namespace Test;

/// <summary>
/// Provides the set of build rules for the project
/// </summary>
public static class BuildRules
{
	private static readonly RuleBuilder _ccRuleBuilder = new RuleBuilder()
		.WithExecutable("clang")
		.WithResponse()
		.AddArguments("-c")
		.AddFlags()
		.AddArguments("-o")
		.AddPrimaryOutput(".o")
		.AddArguments("-MMD", "-MF")
		.AddDepends(".d")
		.AddInput();

	private static readonly RuleBuilder _ldRuleBuilder = new RuleBuilder()
		.WithExecutable("clang")
		.WithResponse()
		.AddArguments("-o")
		.AddPrimaryOutput(".exe")
		.AddInput();

	/// <summary>
    /// Gets the compile rule for the project
    /// </summary>
	public static readonly BuildRule CompileRule = _ccRuleBuilder.CreateRule();

	/// <summary>
    /// Gets the link rule for the project
    /// </summary>
	public static readonly BuildRule LinkRule = _ldRuleBuilder.CreateRule();
}