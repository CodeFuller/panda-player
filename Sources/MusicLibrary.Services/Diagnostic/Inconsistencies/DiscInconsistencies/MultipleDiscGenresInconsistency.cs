using System;
using System.Linq;
using MusicLibrary.Core.Models;
using static CF.Library.Core.Extensions.FormattableStringExtensions;

namespace MusicLibrary.Services.Diagnostic.Inconsistencies.DiscInconsistencies
{
	internal class MultipleDiscGenresInconsistency : BasicDiscInconsistency
	{
		public override string Description => Current($"Disc '{DiscDisplayTitle}' contains different genres: {String.Join(", ", Disc.ActiveSongs.Select(s => s.Genre.Name).Distinct())}");

		public override InconsistencySeverity Severity => InconsistencySeverity.Medium;

		public MultipleDiscGenresInconsistency(DiscModel disc)
			: base(disc)
		{
		}
	}
}
