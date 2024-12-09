using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using CeetemSoft.Utils;

namespace CeetemSoft.UcBuild;

/// <summary>
/// Provides a means to execute build targets intended for use within a C or C++ header based
/// project
/// </summary>
public sealed class MicroBuild
{
	/// <summary>
    /// Provides the shape of the of the executing callback. See the <see cref="Executing"/>
    /// property for more details.
    /// </summary>
	public delegate void ExecutingFunction();

	private int _mthreads;

	/// <summary>
    /// Creates a new build
    /// </summary>
	public MicroBuild()
	{
		_mthreads = 1;
	}

	/// <summary>
    /// Executes a build
    /// </summary>
    /// <param name="root">
    /// The root target of the build to execute
    /// </param>
    /// <returns>
    /// The result of the build execution
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the target dependency graph contains a cycle
    /// </exception>
    /// <remarks>
    /// A build is executed in the following phases:
    /// <list type="number">
	/// <item>
	/// <description>
	/// During the prepare phase, a target dependency graph is created. The graph is then traversed
    /// and each target is queried through its <see cref="BuildTarget.Prepare"/> function to
    /// determine if the target is outdated and needs to be executed. User code can mark additional
    /// targets as outdated by calling the <see cref="BuildTarget.SetOutdated"/> function on a
    /// target. After the graph is traversed, any targets whose dependencies are outdated will also
    /// be marked outdated. Targets are queued and queried in parallel during this phase.
	/// </description>
	/// </item>
    /// <item>
    /// During the execute phase, all targets that are outdated are scheduled to be executed in
    /// dependency order. Any targets whose dependencies are up to date are scheduled to execute
    /// in parallel.
    /// </item>
    /// </list>
    /// </remarks>
	public BuildResult Execute(BuildTarget? root)
	{
		// Get the number of targets we need to build
		if ((root == null) || !Prepare(root, out var targets))
		{
			return default;
		}

		// Invoke the executing callback
		Executing?.Invoke();

		// Execute the outdated targets
		return Execute(targets);
	}

	private BuildResult Execute(List<BuildTarget> targets)
	{
		// Create synchronized input and output collections
		BlockingCollection<BuildTarget?> inputs  = new(new ConcurrentBag<BuildTarget?>());
		BlockingCollection<BuildTarget>  outputs = new(new ConcurrentBag<BuildTarget>());

		// Start the execute workers
		Thread[] threads = ThreadUtils.StartThreads(
			Math.Min(targets.Count, MaxThreads),
			() => ExecuteWorker(inputs, outputs));

		// Cache total jobs at the beginning of the phase
		int totalJobs = targets.Count;

		// Continually schedule jobs in the main thread
		while (targets.Count > 0)
		{
			// Schedule targets
			ScheduleTargets(targets, inputs);

			// Process completed targets
			if (!ProcessCompletedTargets(outputs))
			{
				break;
			}
		}

		// Wait for the threads to complete
		JoinExecuteWorkers(threads, inputs);

		// Process any failed targets
		ProcessFailedtargets(outputs);

		// Create the result
		return new()
		{
			TotalJobs     = totalJobs,
			CompletedJobs = totalJobs - targets.Count
		};
	}

	private bool Prepare(BuildTarget root, out List<BuildTarget> targets)
	{
		// Create a new list of targets
		targets = [];

		// Sort the targets
		SortTargets(root, targets, [], []);

		// Create a synchronized collection to store the targets
		ConcurrentBag<BuildTarget> stargets = new(targets);

		// Start the outdated worker threads
		Thread[] threads = ThreadUtils.StartThreads(
			Math.Min(targets.Count, MaxThreads),
			() => PrepareWorker(stargets));

		// Wait for the threads to complete
		ThreadUtils.JoinThreads(threads);

		// Mark additional targets as outdated
		MarkOutdatedTargets(targets);

		// Filter the list for only outdated targets
		targets = targets.Where(target => target.IsOutdated).ToList();

		// Determine if any targets need to be executed
		return targets.Count > 0;
	}

	private static void ProcessFailedtargets(BlockingCollection<BuildTarget> outputs)
	{
		if (outputs.Count == 0)
		{
			return;
		}

		// Sort the targets to put failed jobs at the end
		BuildTarget[] targets = [.. outputs.OrderBy(target => target.IsOutdated)];

		// Invoke the executed callback on all the targets
		foreach (BuildTarget target in targets)
		{
			target.Executed();
		}		
	}

	private static bool ProcessCompletedTargets(BlockingCollection<BuildTarget> outputs)
	{
		// Wait until there is at least one completed job
		if (!ProcessCompletedTarget(outputs.Take(), outputs))
		{
			return false;
		}

		// Process any pending completed targets
		while (outputs.TryTake(out BuildTarget? target))
		{
			if (!ProcessCompletedTarget(target, outputs))
			{
				return false;
			}
		}

		return true;
	}

	private static bool ProcessCompletedTarget(
		BuildTarget target, BlockingCollection<BuildTarget> outputs)
	{
		// If the target failed, stuff it back into the outputs
		if (target.IsOutdated)
		{
			outputs.Add(target);
			return false;
		}

		// Invoke the executed callback
		target.Executed();

		// The target executed successfully
		return true;
	}

	private static void ScheduleTargets(
		List<BuildTarget> targets, BlockingCollection<BuildTarget?> inputs)
	{
		// Iterate in reverse order so we can remove scheduled targets
		for (int idx = targets.Count - 1; idx >= 0; idx--)
		{
			BuildTarget target = targets[idx];

			if (!CanScheduleTarget(target))
			{
				continue;
			}

			// Queue the target for execution
			inputs.Add(target);

			// Remove the target from the list
			targets.RemoveAt(idx);
		}
	}

	private static bool CanScheduleTarget(BuildTarget target)
	{
		if (target.HasDependencies)
		{
			foreach(BuildTarget dependency in target.Dependencies)
			{
				if (dependency.IsOutdated)
				{
					return false;
				}
			}
		}

		return true;
	}

	private static void MarkOutdatedTargets(List<BuildTarget> targets)
	{
		foreach (BuildTarget target in targets)
		{
			if (!target.HasDependencies)
			{
				continue;
			}

			foreach (BuildTarget dependency in target.Dependencies)
			{
				target.IsOutdated |= dependency.IsOutdated;
			}
		}
	}

	private static void PrepareWorker(ConcurrentBag<BuildTarget> targets)
	{
		while (targets.TryTake(out BuildTarget? target))
		{
			if (target.Prepare())
			{
				target.IsOutdated = true;
			}
		}
	}

	private static void ExecuteWorker(
		BlockingCollection<BuildTarget?> inputs,
		BlockingCollection<BuildTarget> outputs)
	{
		BuildTarget? target;

		// A null target indicates that there are no more targets to process
		while ((target = inputs.Take()) != null)
		{
			// Execute the target
			target.IsOutdated = !target.Execute();

			// Add the target to the outputs
			outputs.Add(target);

			// Exit the target if the target failed
			if (target.IsOutdated)
			{
				break;
			}
		}
	}

	private static void SortTargets(
		BuildTarget target,
		List<BuildTarget> targets,
		HashSet<BuildTarget> visited,
		HashSet<BuildTarget> stacked)
	{
		// Clear outdated flag
		target.IsOutdated = false;

		// Check for graph cycles
		if (stacked.Contains(target))
		{
			ThrowCyclicDependency();
		}

		// Only visit the node once
		else if (visited.Contains(target))
		{
			return;
		}

		visited.Add(target);

		if (target.HasDependencies)
		{
			stacked.Add(target);

			foreach (BuildTarget dependency in target.Dependencies)
			{
				// Recursively sort dependencies
				SortTargets(dependency, targets, visited, stacked);
			}

			stacked.Remove(target);
		}

		targets.Add(target);
	}

	private static void JoinExecuteWorkers(
		Thread[] threads, BlockingCollection<BuildTarget?> inputs)
	{
		// Stuff nulls into the input collection to terminate any running threads
		for (int idx = 0; idx < threads.Length; idx++)
		{
			inputs.Add(null);
		}

		// Wait for the threads to complete
		ThreadUtils.JoinThreads(threads);
	}

	[DoesNotReturn]
	private static void ThrowCyclicDependency()
	{
		throw new InvalidOperationException("The graph of targets contains a cyclic dependency.");
	}

	/// <summary>
    /// Gets or sets the maximum number of threads that can be used for jobs during the build
    /// </summary>
    /// <remarks>
    /// This property is clamped between 1 and <see cref="Environment.ProcessorCount"/>.
    /// </remarks>
	public int MaxThreads
	{
		get => _mthreads;
		set => _mthreads = Math.Clamp(value, 1, Environment.ProcessorCount);
	}

	/// <summary>
    /// Gets or sets the callback to be invoked after the prepare phase of the build
    /// </summary>
	public ExecutingFunction? Executing { get; set; }
}