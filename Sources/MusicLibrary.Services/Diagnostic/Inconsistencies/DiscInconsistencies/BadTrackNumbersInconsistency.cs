using System;
using System.Linq;
using MusicLibrary.Core.Models;
using static CF.Library.Core.Extensions.FormattableStringExtensions;

namespace MusicLibrary.Services.Diagnostic.Inconsistencies.DiscInconsistencies
{
	internal class BadTrackNumbersInconsistency : BasicDiscInconsistency
	{
		public override string Description => Current($"Disc '{DiscDisplayTitle}' contains bad track numbers: {String.Join(", ", Disc.ActiveSongs.Select(s => s.TrackNumber))}");

		public override InconsistencySeverity Severity => InconsistencySeverity.Medium;

		public BadTrackNumbersInconsistency(DiscModel disc)
			: base(disc)
		{
		}
	}
}
