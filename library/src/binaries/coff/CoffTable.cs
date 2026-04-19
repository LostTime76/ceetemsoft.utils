using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace CeetemSoft.Binaries.Coff;

/// <summary>
/// Provides a collection that can be used to access entries within a coff image by name or index
/// </summary>
/// <typeparam name="T"></typeparam>
public sealed class CoffTable<T> : IReadOnlyDictionary<string, T> where T : CoffEntry
{
	internal const int DefaultCapacity = 1024;

	private readonly OrderedDictionary<string, T> _entries;

	internal CoffTable(int initialCapacity = DefaultCapacity)
	{
		_entries = new(initialCapacity);
	}

	/// <summary>
	/// Gets a value that indicates if an entry with a given name is within the table
	/// </summary>
	/// <param name="name">
	/// The name of the entry to check
	/// </param>
	/// <returns>
	/// True if the entry with the given name is within the table, false otherwise
	/// </returns>
	public bool ContainsKey(string name) => _entries.ContainsKey(name);

	/// <summary>
	/// Attempts to retrieve an entry with a given name from the table
	/// </summary>
	/// <param name="name">
	/// The name of the entry to retrieve
	/// </param>
	/// <param name="value">
	/// The entry if it was retrieved successfully, otherwise null
	/// </param>
	/// <returns>
	/// True if the entry was retrieved successfully, false otherwise
	/// </returns>
	public bool TryGetValue(string name, [MaybeNullWhen(false)]out T value)
		=> _entries.TryGetValue(name, out value);

	/// <summary>
	/// Gets an enumerator for the table
	/// </summary>
	/// <returns>
	/// An enumerator for the table
	/// </returns>
	public IEnumerator<KeyValuePair<string, T>> GetEnumerator() => _entries.GetEnumerator();

	/// <summary>
	/// Gets an enumerator for the table
	/// </summary>
	/// <returns>
	/// An enumerator for the table
	/// </returns>
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	internal bool TryAdd(T entry) => _entries.TryAdd(entry.Name, entry);

	/// <summary>
	/// Gets the number of entries within the table
	/// </summary>
	public int Count => _entries.Count;

	/// <summary>
	/// Gets an enumerable of all the names of entries within the table
	/// </summary>
	public IEnumerable<string> Keys => _entries.Keys;

	/// <summary>
	/// Gets an enumerable of all the entries within the table
	/// </summary>
	public IEnumerable<T> Values => _entries.Values;

	/// <summary>
	/// Retrieves an entry by index
	/// </summary>
	/// <param name="index">
	/// The index of the entry to retrieve
	/// </param>
	/// <returns>
	/// The entry that was retrieved
	/// </returns>
	public T this[int index] => _entries.GetAt(index).Value;

	/// <summary>
	/// Retrieves an entry by name
	/// </summary>
	/// <param name="name">
	/// The name of the entry to retrieve
	/// </param>
	/// <returns>
	/// The entry that was retrieved
	/// </returns>
	public T this[string name] => _entries[name];
}