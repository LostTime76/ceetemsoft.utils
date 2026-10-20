namespace CeetemSoft.Utils;

/// <summary>
/// Provides status flags associated with basic git information
/// </summary>
public readonly struct GitInfoFlags
{
	private const int DirtyFlag    = 0x01;
	private const int AheadByShift = 1;
	private const int AheadByMask  = unchecked((int)0xFFFF_FFFE);

	private readonly int _data;

	/// <summary>
	/// Creates new flags from raw integer data
	/// </summary>
	/// <param name="data">
	/// The raw integer data
	/// </param>
	public GitInfoFlags(int data)
	{
		_data = data;
	}

	/// <summary>
	/// Creates new flags
	/// </summary>
	/// <param name="isDirty">
	/// The value that indicates if the repository has local uncomitted changes
	/// </param>
	/// <param name="aheadBy">
	/// The number of commits that have not yet been pushed to the repository
	/// </param>
	public GitInfoFlags(bool isDirty, int aheadBy)
	{
		_data = (isDirty ? DirtyFlag : 0) | (aheadBy << AheadByShift);
	}

	/// <summary>
	/// Implicitly converts flags to raw integer data
	/// </summary>
	/// <param name="flags">
	/// The flags to convert
	/// </param>
	public static implicit operator int(GitInfoFlags flags) => flags._data;

	/// <summary>
	/// Implicitly converts raw integer data to flags
	/// </summary>
	/// <param name="data">
	/// The raw integer data to convert
	/// </param>
	public static implicit operator GitInfoFlags(int data) => new(data);

	/// <summary>
	/// Gets a value that indicates if the repository has local uncomitted changes
	/// </summary>
	public bool IsDirty => (_data & DirtyFlag) != 0;

	/// <summary>
	/// Gets a value that indicates the number of commits that have not yet been pushed to the
	/// repository
	/// </summary>
	public int AheadBy => (_data & AheadByMask) >> AheadByShift;
}