using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MusicLibrary.Core.Comparers;
using MusicLibrary.Core.Models;
using MusicLibrary.Services.Diagnostic.Inconsistencies;
using MusicLibrary.Services.Diagnostic.Inconsistencies.DiscInconsistencies;
using MusicLibrary.Services.Diagnostic.Interfaces;
using MusicLibrary.Services.Interfaces;

namespace MusicLibrary.Services.Diagnostic.Checkers
{
	internal class DiscConsistencyChecker : IDiscConsistencyChecker
	{
		private readonly IDiscTitleToAlbumMapper discTitleToAlbumMapper;

		public DiscConsistencyChecker(IDiscTitleToAlbumMapper discTitleToAlbumMapper)
		{
			this.discTitleToAlbumMapper = discTitleToAlbumMapper ?? throw new ArgumentNullException(nameof(discTitleToAlbumMapper));
		}

		public Task CheckDiscsConsistency(IEnumerable<DiscModel> discs, Action<LibraryInconsistency> inconsistenciesHandler, CancellationToken cancellationToken)
		{
			foreach (var disc in discs)
			{
				var albumTitle = discTitleToAlbumMapper.GetAlbumTitleFromDiscTitle(disc.Title);

				// Checking disc title
				if (!String.Equals(albumTitle, disc.AlbumTitle, StringComparison.Ordinal))
				{
					inconsistenciesHandler(new SuspiciousAlbumTitleInconsistency(disc));
				}

				var songs = disc.ActiveSongs.ToList();

				// Checking songs order & track numbers
				var trackNumbers = songs.Select(s => s.TrackNumber).ToList();
				if (trackNumbers.Any(n => n != null))
				{
					if (trackNumbers.Any(n => n == null) || !Enumerable.Range(1, trackNumbers.Count).SequenceEqual(trackNumbers.Select(t => (int)t.Value)))
					{
						inconsistenciesHandler(new BadTrackNumbersInconsistency(disc));
					}
				}

				// Checking that all disc songs have equal genre
				var genres = songs.Select(s => s.Genre).Distinct(new GenreEqualityComparer()).ToList();
				if (genres.Count > 1)
				{
					inconsistenciesHandler(new MultipleDiscGenresInconsistency(disc));
				}
			}

			return Task.CompletedTask;
		}
	}
}
