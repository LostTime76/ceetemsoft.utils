namespace CeetemSoft.Runtime;

/// <summary>
/// Provides a set of console extension members
/// </summary>
public static class ConsoleExtensions
{
	/// <summary>
    /// Writes a line of text to the console if the text is not empty
    /// </summary>
    /// <param name="text">
    /// The text to write to the console
    /// </param>
	public static void WriteLineIfNotEmpty(string? text)
	{
		if (!string.IsNullOrEmpty(text))
		{
			Console.WriteLine(text);
		}
	}
}