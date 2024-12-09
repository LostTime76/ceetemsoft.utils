namespace CeetemSoft.UcBuild;

/// <summary>
/// Provides a means to take a set of inputs and produce a set of outputs for use within a build
/// </summary>
public class BuildTarget
{
	private List<BuildTarget>? _dependencies;

	/// <summary>
	/// Used to process status information after a target has executed
	/// </summary>
	/// <remarks>
	/// This callback is invoked unconditionally for each target that was executed during a build.
	/// The invocation is buffered, that is each invocation per target is called serially. A
	/// typical implementation will use this callback to process status information for targets
	/// that execute in parallel, such as printing to the console.
	/// </remarks>
	protected internal virtual void Executed() { }

	/// <summary>
	/// Determines if a target is outdated and prepares it for execution
	/// </summary>
	/// <returns>
	/// True if the target is outdated, false otherwise
	/// </returns>
	/// <remarks>
	/// This function is called unconditionally for each target during the prepare phase of a build
	/// and the implementation must be thread safe. The default implementation returns false.
	/// </remarks>
	protected internal virtual bool Prepare() => false;

	/// <summary>
    /// Takes a set of inputs and produces a set out outputs within a build
    /// </summary>
    /// <returns>
    /// True if the target executed successfully, false otherwise
    /// </returns>
    /// <remarks>
    /// This function is called for each outdated target during the execute phase of a build and
    /// the implementation generally must be thread safe. The default implementation returns true.
    /// </remarks>
	protected internal virtual bool Execute() => true;

	/// <summary>
	/// Marks the target as outdated
	/// </summary>
	/// <remarks>
	/// This function can be called by user code during the prepare phase of a build to mark
	/// additional targets as outdated.
	/// </remarks>
	public void SetOutdated() => IsOutdated = true;

	/// <summary>
    /// Gets a value that indicates if the target is outdated and needs to be executed
    /// </summary>
	public bool IsOutdated { get; internal set; }

	/// <summary>
    /// Gets a value that indicates if the target has any dependencies
    /// </summary>
	public bool HasDependencies => (_dependencies != null) && (_dependencies.Count > 0);

	/// <summary>
	/// Gets the list of dependencies for the target
	/// </summary>
	/// <remarks>
	/// As most targets will not have dependencies, this property is lazily instantiated on demand
	/// upon access. Instead of accessing this property to determine if the target has any
	/// dependencies, opt to use the <see cref="HasDependencies"/> property instead as it won't
	/// cause an instantiation.
	/// </remarks>
	public List<BuildTarget> Dependencies => _dependencies ??= [];
}