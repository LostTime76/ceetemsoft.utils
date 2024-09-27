using System.Collections.Concurrent;

namespace CeetemSoft.Utils;

/// <summary>
/// Provides a set of extension functions for concurrent collections
/// </summary>
public static class ConcurrentExtensions
{
	/// <summary>
    /// Adds the items within an enumerable to the queue
    /// </summary>
    /// <typeparam name="T">
    /// The generic type of the queue
    /// </typeparam>
    /// <param name="queue">
    /// The queue to add the items to
    /// </param>
    /// <param name="items">
    /// The enumerable containing the items to add to the queue
    /// </param>
	public static void AddRange<T>(this ConcurrentBag<T> queue, IEnumerable<T> items)
	{
		foreach (T item in items)
		{
			queue.Add(item);
		}
	}

    /// <summary>
    /// Adds the items within an enumerable to the queue
    /// </summary>
    /// <typeparam name="T">
    /// The generic type of the queue
    /// </typeparam>
    /// <param name="queue">
    /// The queue to add the items to
    /// </param>
    /// <param name="items">
    /// The enumerable containing the items to add to the queue
    /// </param>
	public static void AddRange<T>(this BlockingCollection<T> queue, IEnumerable<T> items)
	{
		foreach (T item in items)
		{
			queue.Add(item);
		}
	}
}