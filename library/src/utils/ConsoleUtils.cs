namespace CeetemSoft.Utils;

/// <summary>
/// Provides a set of console utility functions
/// </summary>
public static class ConsoleUtils
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