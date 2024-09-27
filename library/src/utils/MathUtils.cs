using System.Runtime.CompilerServices;

namespace CeetemSoft.Utils;

/// <summary>
/// Provides a set of math utility functions
/// </summary>
public static class MathUtils
{
	/// <summary>
    /// Ensures that a given integer value is greater than or equal to a minimum value
    /// </summary>
    /// <param name="value">
    /// The value
    /// </param>
    /// <param name="minimum">
    /// The minimum allowable value
    /// </param>
    /// <returns>
    /// A value that is always greater than or equal to <paramref name="minimum"/>.
    /// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static int AtLeast(int value, int minimum)
	{
		return value >= minimum ? value : minimum;
	}
}