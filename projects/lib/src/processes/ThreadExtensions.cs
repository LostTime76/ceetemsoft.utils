namespace CeetemSoft.Utils;

/// <summary>
/// Provides a set of thread extension members
/// </summary>
public static class ThreadExtensions
{
	/// <summary>
    /// Joins all of the threads within an array
    /// </summary>
    /// <param name="threads">
    /// The array containing the threads to join
    /// </param>
	public static void JoinThreads(Thread[] threads)
	{
		for (int idx = 0; idx < threads.Length; idx++)
		{
			threads[idx].Join();
		}
	}

	/// <summary>
    /// Creates and starts a number of threads that execute a specified function
    /// </summary>
    /// <param name="count">
    /// The number of threads to create and start
    /// </param>
    /// <param name="function">
    /// The function that each thread will execute
    /// </param>
    /// <returns>
    /// An array containing all of the threads that were created and started
    /// </returns>
	public static Thread[] StartThreads(int count, ThreadStart function)
	{
		Thread[] threads = new Thread[count];

		for (int idx = 0; idx < count; idx++)
		{
			Thread thread = new(function);
			threads[idx]  = thread;
			thread.Start();
		}

		return threads;
	}
}