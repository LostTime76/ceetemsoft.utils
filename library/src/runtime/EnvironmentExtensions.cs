using System.Diagnostics.CodeAnalysis;

namespace CeetemSoft.Runtime;

/// <summary>
/// Provides a set of environment extension members
/// </summary>
public static class EnvironmentExtensions
{
	/// <summary>
    /// Gets the value of an environment variable or throws an exception if it is not found
    /// </summary>
    /// <param name="variable">
    /// The environment variable to retrieve the value of
    /// </param>
    /// <returns>
    /// The value of the environment variable
    /// </returns>
    /// <exception cref="KeyNotFoundException">
    /// Thrown if the environment variable was not found
    /// </exception>
	public static string GetVariableOrThrow(string variable)
	{
		string? value = Environment.GetEnvironmentVariable(variable);

		if (value == null)
		{
			ThrowVariableNotFound(variable);
		}

		return value;
	}

	[DoesNotReturn]
	private static void ThrowVariableNotFound(string variable)
	{
		throw new KeyNotFoundException($"The \"{variable}\" environment variable was not found.");
	}
}