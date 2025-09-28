namespace CeetemSoft.Attributes;

/// <summary>
/// Provides an attribute that stores an author
/// </summary>
[AttributeUsage(AttributeTargets.All)]
public class AuthorAttribute : Attribute
{
	/// <summary>
	/// Creates a new author attribute
	/// </summary>
	public AuthorAttribute() { }

	/// <summary>
	/// Creates a new author attribute that stores the given author
	/// </summary>
	/// <param name="author"></param>
	public AuthorAttribute(string? author)
	{
		Author = author;
	}

	/// <summary>
	/// Gets or initializes the author
	/// </summary>
	public string? Author { get; init; }
}