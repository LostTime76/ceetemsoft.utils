namespace CeetemSoft.UcBuild;

public abstract class BuildTarget
{
	public virtual void Executed(int jobIndex, int totalJobs) { }

	public virtual bool CheckIfOutdated() => false;

	public abstract bool Execute();

	public bool IsOutdated { get; internal set; }
}