namespace CeetemSoft.Utils;

/// <summary>
/// Provides an attribute that can be set on an assembly to describe the time it was build
/// </summary>
[AttributeUsage(AttributeTargets.Assembly)]
public sealed class BuildTimeAttribute : Attribute
{
	/// <summary>
	/// Creates a new attribute with using the current utc time
	/// </summary>
	public BuildTimeAttribute()
	{
		BuildTime = DateTime.UtcNow;
	}

	/// <summary>
	/// Gets the time that the assembly was built
	/// </summary>
	public DateTime BuildTime { get; private init; }
}