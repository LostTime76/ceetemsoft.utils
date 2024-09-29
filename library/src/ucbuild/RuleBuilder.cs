using CeetemSoft.Utils;

namespace CeetemSoft.UcBuild;

public sealed class RuleBuilder
{
	public const string DefaultResponseArgument = "@";

	public const string DefaultResponseExt = ".rsp";

	private readonly List<object?> _arguments;
	private readonly List<string>  _outExtsl;

	private bool      _frozen;
	private int       _flagsIdx;
	private string?   _executable;
	private string?   _rspExt;
	private string?   _rspFormat;
	private string?   _poutExt;
	private string?   _depsExt;
	private string[]? _outExts;

	public RuleBuilder()
	{
		_flagsIdx  = -1;
		_arguments = [];
		_outExtsl  = [];
	}
	
	public RuleBuilder WithExecutable(string? executable)
	{
		AssertThawed();
		AssertValidExecutable(executable);
		_executable = executable;
		return this;
	}

	public RuleBuilder WithResponse(
		string argument = DefaultResponseArgument,
		string extension = DefaultResponseExt)
	{
		AssertThawed();
		AssertValidResponse(argument, extension);
		_rspFormat = $"{argument}{{0}}";
		_rspExt    = extension;
		return this;
	}

	public RuleBuilder AddInput()
	{
		AssertThawed();
		_arguments.Add("{0}");
		return this;
	}

	public RuleBuilder AddPrimaryOutput(string extension)
	{
		AssertThawed();
		AssertSinglePrimaryOutput();
		_poutExt = AddOutputExtension(extension);
		return this;
	}

	public RuleBuilder AddSecondaryOutput(string extension)
	{
		AssertThawed();
		AssertSingleDepends();
		AddOutputExtension(extension);
		return this;
	}

	public RuleBuilder AddDepends(string extension)
	{
		AssertThawed();
		_depsExt = AddOutputExtension(extension);
		return this;
	}

	public RuleBuilder AddFlags()
	{
		AssertThawed();

		if (_flagsIdx != -1)
		{
			ThrowDuplicateFlags();
		}

		_flagsIdx = _arguments.Count;
		_arguments.Add(null);
		return this;
	}

	public RuleBuilder AddArguments(params string[] arguments)
	{
		return AddArguments((IEnumerable<string>)arguments);
	}

	public RuleBuilder AddArguments(IEnumerable<string> arguments)
	{
		AssertThawed();
		_arguments.AddRange(arguments);
		return this;
	}

	public BuildRule CreateRule()
	{
		return CreateRule(null);
	}

	public BuildRule CreateRule(IEnumerable<string?>? flags)
	{
		// Assert that the rule is valid
		AssertValidRule(flags);

		// Mark the builder frozen
		_frozen = true;

		// Create the rule
		return new()
		{
			Executable       = _executable!,
			ArgumentFormat   = GetArgumentFormat(flags),
			PrimaryOutputExt = _poutExt!,
			ResponseFormat   = _rspFormat,
			ResponseExt      = _rspExt,
			DependsExt       = _depsExt,
			OutputExts       = _outExts ??= [.._outExtsl]
		};
	}

	private string AddOutputExtension(string extension)
	{
		if (!FileUtils.IsValidExtension(extension))
		{
			ThrowInvalidOutputExtension(extension);
		}
		else if (_outExtsl.Contains(extension))
		{
			ThrowDuplicateOutputExtension(extension);
		}

		_outExtsl.Add(extension);
		_arguments.Add($"{{{_outExtsl.Count}}}");
		return extension;
	}

	private string GetArgumentFormat(IEnumerable<string?>? flags)
	{
		// Set the flags within the argument array if there are any
		if (_flagsIdx != -1)
		{
			_arguments[_flagsIdx] = flags;
		}

		// Join all of the arguments
		return Sclex.Join(_arguments);
	}

	private void AssertThawed()
	{
		if (_frozen)
		{
			ThrowFrozen();
		}
	}

	private void AssertValidExecutable(string? executable)
	{
		if (StringUtils.IsEmpty(executable))
		{
			ThrowInvalidExecutable();
		}
		else if (_executable != null)
		{
			ThrowDuplicateExecutable();
		}
	}

	private void AssertValidResponse(string argument, string extension)
	{
		if (_rspFormat != null)
		{
			ThrowDuplicateResponse();
		}
		else if (StringUtils.IsEmpty(argument))
		{
			ThrowInvalidResponseArgument(argument);
		}
		else if (!FileUtils.IsValidExtension(extension))
		{
			ThrowInvalidResponseExtension(extension);
		}
	}

	private void AssertSinglePrimaryOutput()
	{
		if (_poutExt != null)
		{
			ThrowDuplicatePrimaryOutput();
		}
	}

	private void AssertSingleDepends()
	{
		if (_depsExt != null)
		{
			ThrowDuplicateDepends();
		}
	}

	private void AssertValidRule(IEnumerable<string?>? flags)
	{
		if (StringUtils.IsEmpty(_executable))
		{
			ThrowInvalidExecutable();
		}
		else if (StringUtils.IsEmpty(_poutExt))
		{
			ThrowInvalidPrimaryOutput();
		}
		else if ((flags != null) && (_flagsIdx == -1))
		{
			ThrowFlagsNotSpecified();
		}
	}

	private static void ThrowFrozen()
	{
		throw new InvalidOperationException("The builder has been frozen and does not allow any " +
			"further modifications.");
	}

	private static void ThrowInvalidResponseExtension(string? extension)
	{
		throw new ArgumentException(extension, nameof(extension));
	}

	private static void ThrowInvalidOutputExtension(string? extension)
	{
		throw new ArgumentException(extension, nameof(extension));
	}

	private static void ThrowDuplicateOutputExtension(string extension)
	{
		throw new ArgumentException(extension, nameof(extension));
	}

	private static void ThrowInvalidExecutable()
	{
		throw new InvalidOperationException("A valid executable must be specified.");
	}

	private static void ThrowInvalidResponseArgument(string argument)
	{
		throw new ArgumentException(argument, nameof(argument));
	}

	private static void ThrowInvalidPrimaryOutput()
	{
		throw new InvalidOperationException("A valid primary output must be specified.");
	}

	private static void ThrowDuplicateExecutable()
	{
		throw new InvalidOperationException("An executable was alreaded added to the builder.");
	}

	private static void ThrowDuplicateResponse()
	{
		throw new InvalidOperationException("Response file options were already added to the " +
			"builder.");
	}

	private static void ThrowDuplicatePrimaryOutput()
	{
		throw new InvalidOperationException("A primary output was already added to the builder.");
	}

	private static void ThrowDuplicateDepends()
	{
		throw new InvalidOperationException("A dependency output was already added to the " +
			"builder.");
	}

	private static void ThrowFlagsNotSpecified()
	{
		throw new InvalidOperationException("Flags were given to create the rule, but no flags" +
			"were added to the builder.");
	}

	private static void ThrowDuplicateFlags()
	{
		throw new InvalidOperationException("Flags were already added to the builder.");
	}

	/// <summary>
    /// Gets a value that indicates if the builder is frozen, that is it does not allow any further
    /// modifications.
    /// </summary>
	public bool IsFrozen => _frozen;
}