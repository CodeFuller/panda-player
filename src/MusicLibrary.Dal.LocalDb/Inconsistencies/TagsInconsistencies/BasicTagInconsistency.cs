using System;
using MusicLibrary.Core.Models;
using MusicLibrary.Services.Diagnostic.Inconsistencies;

namespace MusicLibrary.Dal.LocalDb.Inconsistencies.TagsInconsistencies
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
