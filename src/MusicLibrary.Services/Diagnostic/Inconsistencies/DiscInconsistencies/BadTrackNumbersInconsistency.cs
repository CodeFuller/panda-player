using System;
using System.Linq;
using MusicLibrary.Core.Models;

namespace MusicLibrary.Services.Diagnostic.Inconsistencies.DiscInconsistencies
{
	internal class BadTrackNumbersInconsistency : BasicDiscInconsistency
	{
		public override string Description => $"Disc '{DiscDisplayTitle}' contains bad track numbers: {String.Join(", ", Disc.ActiveSongs.Select(s => s.TrackNumber))}";

		public override InconsistencySeverity Severity => InconsistencySeverity.Medium;

		public BadTrackNumbersInconsistency(DiscModel disc)
			: base(disc)
		{
		}
	}
}
