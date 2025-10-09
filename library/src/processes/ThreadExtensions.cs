namespace CeetemSoft.Processes;

/// <summary>
/// Provides a set of thread extension members
/// </summary>
public static class ThreadExtensions
{
	/// <summary>
	/// Joins all the threads within an enumerable
	/// </summary>
	/// <param name="threads">
	/// The enumerable containing the threads to join
	/// </param>
	public static void Join(this IEnumerable<Thread> threads)
	{
		foreach(Thread thread in threads)
		{
			thread.Join();
		}
	}

	/// <summary>
	/// Provides a set of thread extension members
	/// </summary>
	extension(Thread)
	{
		/// <summary>
		/// Creates and starts a thread that executes a specified function
		/// </summary>
		/// <param name="function">
		/// The function that the thread will execute
		/// </param>
		/// <returns>
		/// The thread that was created and started
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="function"/> is null
		/// </exception>
		public static Thread Start(ThreadStart function) => Thread.StartMany(1, function)[0];

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
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="count"/> is less than 0
		/// </exception>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="function"/> is null
		/// </exception>
		public static Thread[] StartMany(int count, ThreadStart function)
		{
			ArgumentNullException.ThrowIfNull(function, nameof(function));
			ArgumentOutOfRangeException.ThrowIfLessThan(count, 0, nameof(count));

			Thread[] threads = new Thread[count];

			for (int idx = 0; idx < count; idx++)
			{
				Thread thread = new(function);
				threads[idx] = thread;
				thread.Start();
			}

			return threads;
		}
	}
}