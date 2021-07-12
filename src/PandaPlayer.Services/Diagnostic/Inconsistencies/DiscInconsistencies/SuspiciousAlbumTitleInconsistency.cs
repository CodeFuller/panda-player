using PandaPlayer.Core.Models;

namespace PandaPlayer.Services.Diagnostic.Inconsistencies.DiscInconsistencies
{
	internal class SuspiciousAlbumTitleInconsistency : BasicDiscInconsistency
	{
		public override string Description => $"Album title looks suspicious for disc '{DiscDisplayTitle}': '{Disc.AlbumTitle}'";

		public override InconsistencySeverity Severity => InconsistencySeverity.Medium;

		public SuspiciousAlbumTitleInconsistency(DiscModel disc)
			: base(disc)
		{
		}
	}
}
