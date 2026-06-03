namespace CeetemSoft.Utils;

/// <summary>
/// Provides a set of environment extension members
/// </summary>
public static class EnvironmentExtensions
{
	/// <summary>
	/// Gets the username environment variable value
	/// </summary>
	public const string UsernameVariable = "USERNAME";

	extension(Environment)
	{
		/// <summary>
		/// Gets the value of the username environment variable
		/// </summary>
		/// <returns>
		/// The value of the username environment variable
		/// </returns>
		public static string? GetUsername()
			=> Environment.GetEnvironmentVariable(UsernameVariable);
	}
}