using CeetemSoft.Utils;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace CeetemSoft.UcBuild;

/// <summary>
/// Provides a thread safe database that targets can use to determine if a source file is outdated
/// with respect to any of its header dependencies changing
/// </summary>
public sealed partial class DependsDatabase
{
	/// <summary>
    /// Gets a best guess number of header dependencies per source file that can be used to estimate
    /// the number of entries within the database
    /// </summary>
	public const int DefaultHeadersPerSource = 16;

	/// <summary>
    /// The default header file extensions within a header dependency makefile
    /// </summary>
	public static readonly HashSet<string> DefaultHeaderFileExts = [".h", ".hh", "hpp"];

	private static readonly JsonTypeInfo<Entry[]> EntryArrayJsonType =
		DependsDatabaseJsonContext.Default.EntryArray;

	private readonly HashSet<string>          _headerExts;
	private readonly Dictionary<string, long> _rtable;
	private readonly Dictionary<string, long> _otable;

	/// <summary>
	/// Creates a new database
	/// </summary>
    /// <param name="filepath">
    /// The path to the database to load
    /// </param>
	/// <param name="headerFileExts">
	/// An enumerable containing the header file extensions within the dependency file. If the
    /// argument is null, <see cref="DefaultHeaderFileExts"/> is used instead.
	/// </param>
	public DependsDatabase(string filepath,	HashSet<string>? headerFileExts = null)
	{
		_headerExts = headerFileExts ?? DefaultHeaderFileExts;
		_rtable     = Load(filepath);
		_otable     = [];
	}

	/// <summary>
    /// Saves the database to a file
    /// </summary>
    /// <param name="filepath">
    /// The path of the database file
    /// </param>
	public void Save(string filepath)
	{
		// Create the database entries
		Entry[] entries = CreateEntries(_otable);

		// Serialize the entries
		string json = JsonSerializer.Serialize(entries, EntryArrayJsonType);

		// Save the entries
		FileUtils.WriteAllTextIfDifferent(filepath, json);
	}

	/// <summary>
    /// Updates the database with the header dependencies of a source file
    /// </summary>
    /// <param name="filepath">
    /// The path to the header dependency file for the source file. The file must be in a makefile
    /// header dependency format.
    /// </param>
    /// <remarks>
    /// This function is thread safe and will typically be used to update the header dependencies
    /// of a source file after a compilation.
    /// </remarks>
	public void UpdateDepends(string filepath)
	{
		AddEntries(filepath);
	}

	/// <summary>
    /// Gets a value indicating if the header dependencies of a source file are outdated with
    /// respect to the source file
    /// </summary>
    /// <param name="filepath">
    /// The path to the header dependency file for the source file. The file must be in a makefile
    /// header dependency format.
    /// </param>
    /// <param name="timestamp">
    /// The timestamp of the source file
    /// </param>
    /// <returns>
    /// True if any of the header dependencies are outdated, false otherwise
    /// </returns>
    /// <remarks>
    /// This function is thread safe and will generally be used to determine if a source file needs
    /// to be rebuilt due to any of its header file dependencies changing.
    /// </remarks>
	public bool AreDependsOutdated(string filepath, long timestamp)
	{
		// Ensure the depedencies are within the output table
		string[] depends = AddEntries(filepath);

		// Check if each dependency is outdated
		for (int idx = 0; idx < depends.Length; idx++)
		{
			string depend = depends[idx];

			if (!_rtable.TryGetValue(depend, out long otimestamp) ||
				(otimestamp != _otable[depend]))
			{
				return true;
			}
		}

		return false;
	}

	private string[] ReadDepends(string filepath)
	{
		List<string>     depends = [];
		SpanReader<char> reader  = new(File.ReadAllText(filepath));
		StringBuilder    text    = new();
		string?          depend;

		while ((depend = ReadDepend(ref reader, text)) != null)
		{
			depends.Add(depend);
		}

		return [.. depends];
	}

	private string? ReadDepend(ref SpanReader<char> reader, StringBuilder text)
	{
		string? filepath;

		while ((filepath = ReadFilepath(ref reader, text)) != null)
		{
			if (_headerExts.Contains(Path.GetExtension(filepath)))
			{
				return filepath;
			}
		}

		return null;
	}

	private static string? ReadFilepath(ref SpanReader<char> reader, StringBuilder text)
	{
		char curr;

		if (!SkipWhitespace(ref reader))
		{
			return null;
		}

		text.Clear();

		while ((curr = reader.Current) != 0)
		{
			switch(curr)
			{
				case '\n':
				case '\r':
				case '\t':
				case ' ':
					goto Done;
				case '\\':
					ReadForwardSlash(ref reader, text);
					break;
				default:
					text.Append(curr);
					break;
			}

			reader.Read();
		}

	Done:
		return text.ToString();
	}

	private static void ReadForwardSlash(ref SpanReader<char> reader, StringBuilder text)
	{
		char curr;

		switch(curr = reader.Read())
		{
			case ' ':
				text.Append(' ');
				break;
			default:
				text.Append('\\');
				text.Append(curr);
				break;
		}
	}

	private static bool SkipWhitespace(ref SpanReader<char> reader)
	{
		char curr;

		while ((curr = reader.Read()) != 0)
		{
			switch(curr)
			{
				case '\n':
				case '\r':
				case '\\':
				case '\t':
				case ' ':
					break;
				default:
					return true;
			}
		}

		return false;
	}

	private string[] AddEntries(string filepath)
	{
		string[] depends = ReadDepends(filepath);

		lock(_otable)
		{
			for (int idx = 0; idx < depends.Length; idx++)
			{
				string depend = depends[idx];

				if (!_otable.ContainsKey(depend))
				{
					_otable.Add(depend, FileUtils.GetTimestamp(depend));
				}
			}
		}

		return depends;
	}

	private static Entry[] CreateEntries(Dictionary<string, long> table)
	{
		int     idx     = 0;
		Entry[] entries = new Entry[table.Count];

		foreach (KeyValuePair<string, long> kvp in table)
		{
			entries[idx++] = new() { Filepath = kvp.Key, Timestamp = kvp.Value };
		}

		return entries;
	}

	private static Dictionary<string, long> Load(string filepath)
	{
		if (!JsonUtils.TryDeserialize(filepath, EntryArrayJsonType, out Entry[]? entries))
		{
			return [];
		}

		var table = new Dictionary<string, long>();

		for (int idx = 0; idx < entries.Length; idx++)
		{
			ref Entry entry     = ref entries[idx];
			string?   hfpath    = entry.Filepath;
			long      timestamp = entry.Timestamp;

			// Check for an invalid database
			if ((hfpath == null) || (timestamp == 0))
			{
				return [];
			}

			// The header filepath must be unique
			else if (!table.TryAdd(hfpath, timestamp))
			{
				return [];
			}
		}

		return table;
	}

	internal struct Entry
	{
		[JsonPropertyName("fpath")]
		public string? Filepath { get; set; }

		[JsonPropertyName("ts")]
		public long Timestamp { get; set; }
	}

	[JsonSourceGenerationOptions(WriteIndented = true)]
	[JsonSerializable(typeof(Entry[]))]
	internal partial class DependsDatabaseJsonContext : JsonSerializerContext { }
}