namespace CeetemSoft.Attributes;

/// <summary>
/// Provides an attribute that stores a date and time
/// </summary>
[AttributeUsage(AttributeTargets.All)]
public class TimeAttribute : Attribute
{
	/// <summary>
	/// Creates a new attribute that stores the current time
	/// </summary>
	public TimeAttribute() : this(DateTime.UtcNow) { }

	/// <summary>
	/// Creates a new attribute that stores the given time
	/// </summary>
	/// <param name="time">
	/// The time to store
	/// </param>
	public TimeAttribute(DateTime time)
	{
		Time = time;
	}

	/// <summary>
	/// Gets or initializes the time
	/// </summary>
	public DateTime Time { get; init; }
}