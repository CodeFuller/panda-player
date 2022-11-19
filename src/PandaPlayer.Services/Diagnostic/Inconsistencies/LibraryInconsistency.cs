namespace PandaPlayer.Services.Diagnostic.Inconsistencies
{
	public abstract class LibraryInconsistency
	{
		public abstract string Description { get; }

		public abstract InconsistencySeverity Severity { get; }
	}
}
