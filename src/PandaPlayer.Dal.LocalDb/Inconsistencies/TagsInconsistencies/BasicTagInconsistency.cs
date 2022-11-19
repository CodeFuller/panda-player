using System;
using PandaPlayer.Core.Models;
using PandaPlayer.Services.Diagnostic.Inconsistencies;

namespace PandaPlayer.Dal.LocalDb.Inconsistencies.TagsInconsistencies
{
	internal abstract class BasicTagInconsistency : LibraryInconsistency
	{
		protected SongModel Song { get; }

		protected string SongDisplayTitle => $"{Song.Disc.Folder.Name}/{Song.Disc.TreeTitle}/{Song.TreeTitle}";

		public override InconsistencySeverity Severity => InconsistencySeverity.Medium;

		protected BasicTagInconsistency(SongModel song)
		{
			Song = song ?? throw new ArgumentNullException(nameof(song));
		}
	}
}
