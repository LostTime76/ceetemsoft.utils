using System.Runtime.CompilerServices;

namespace CeetemSoft.Utils;

/// <summary>
/// Provides a set of math utility functions
/// </summary>
public static class MathUtils
{
	/// <summary>
    /// Ensures that a given integer value is greater than or equal to another integer value
    /// </summary>
    /// <param name="value">
    /// The integer value to bound
    /// </param>
    /// <param name="minimum">
    /// The integer value to bound to
    /// </param>
    /// <returns>
    /// If <paramref name="value"/> is greater than or equal to <paramref name="minimum"/>,
    /// <paramref name="value"/> is returned, otherwise <paramref name="minimum"/> is returned.
    /// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static int EnsureGte(int value, int minimum)
	{
		return value >= minimum ? value : minimum;
	}
}