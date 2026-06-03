namespace CeetemSoft.Utils;

/// <summary>
/// Provides an attribute that can be set on an assembly to describe its author
/// </summary>
[AttributeUsage(AttributeTargets.Assembly)]
public sealed class AuthorAttribute : Attribute
{
	/// <summary>
	/// Creates a new attribute with the specified author
	/// </summary>
	/// <param name="author">
	/// The author of the assembly. If the value is null, the username environment variable
	/// is used.
	/// </param>
	public AuthorAttribute(string? author = null)
	{
		Author = author ?? Environment.GetUsername();
	}

	/// <summary>
	/// Gets the author of the assembly
	/// </summary>
	public string? Author { get; private init; }
}