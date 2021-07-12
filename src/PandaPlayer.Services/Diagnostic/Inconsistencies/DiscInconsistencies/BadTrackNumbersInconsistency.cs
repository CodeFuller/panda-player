using System;
using System.Globalization;
using System.Linq;
using PandaPlayer.Core.Models;

namespace PandaPlayer.Services.Diagnostic.Inconsistencies.DiscInconsistencies
{
	internal class BadTrackNumbersInconsistency : BasicDiscInconsistency
	{
		public override string Description => $"Disc '{DiscDisplayTitle}' contains bad track numbers: {String.Join(", ", Disc.ActiveSongs.Select(s => s.TrackNumber?.ToString(CultureInfo.InvariantCulture) ?? "<empty>"))}";

		public override InconsistencySeverity Severity => InconsistencySeverity.Medium;

		public BadTrackNumbersInconsistency(DiscModel disc)
			: base(disc)
		{
		}
	}
}
