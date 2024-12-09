namespace CeetemSoft.UcBuild;

/// <summary>
/// Provides the result of a build run
/// </summary>
public readonly struct BuildResult
{
	/// <summary>
    /// Gets the number of jobs that were outdated and needed to be executed by the build
    /// </summary>
	public int TotalJobs { get; internal init; }

	/// <summary>
    /// Gets the number of jobs that were completed by the build
    /// </summary>
	public int CompletedJobs { get; internal init; }

	/// <summary>
    /// Gets a value indicating if the build was successful
    /// </summary>
	public bool Success => TotalJobs == CompletedJobs;
}