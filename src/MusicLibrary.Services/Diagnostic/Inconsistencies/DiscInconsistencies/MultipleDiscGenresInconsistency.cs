using System;
using System.Linq;
using MusicLibrary.Core.Models;

namespace MusicLibrary.Services.Diagnostic.Inconsistencies.DiscInconsistencies
{
	internal class MultipleDiscGenresInconsistency : BasicDiscInconsistency
	{
		public override string Description => $"Disc '{DiscDisplayTitle}' contains different genres: {String.Join(", ", Disc.ActiveSongs.Select(s => s.Genre?.Name ?? "<empty>").Distinct())}";

		public override InconsistencySeverity Severity => InconsistencySeverity.Medium;

		public MultipleDiscGenresInconsistency(DiscModel disc)
			: base(disc)
		{
		}
	}
}
