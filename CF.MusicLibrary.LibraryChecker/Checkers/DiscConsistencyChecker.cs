using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CF.MusicLibrary.Core;
using CF.MusicLibrary.Core.Objects;
using CF.MusicLibrary.LibraryChecker.Registrators;
using static CF.Library.Core.Application;

namespace CF.MusicLibrary.LibraryChecker.Checkers
{
	public class DiscConsistencyChecker : IDiscConsistencyChecker
	{
		private readonly ILibraryInconsistencyRegistrator inconsistencyRegistrator;

		public DiscConsistencyChecker(ILibraryInconsistencyRegistrator inconsistencyRegistrator)
		{
			if (inconsistencyRegistrator == null)
			{
				throw new ArgumentNullException(nameof(inconsistencyRegistrator));
			}

			this.inconsistencyRegistrator = inconsistencyRegistrator;
		}

		public async Task CheckDiscsConsistency(IEnumerable<Disc> discs)
		{
			Logger.WriteInfo("Checking discs consistency ...");

			foreach (var disc in discs)
			{
				//	Checking album title
				if (DiscTitleToAlbumMapper.AlbumTitleIsSuspicious(disc.AlbumTitle))
				{
					inconsistencyRegistrator.RegisterInconsistency_SuspiciousAlbumTitle(disc);
				}

				//	Check that disc has some songs
				if (!disc.Songs.Any())
				{
					inconsistencyRegistrator.RegisterInconsistency_DiscWithoutSongs(disc);
					continue;
				}

				//	Checking songs order & track numbers
				var trackNumbers = disc.Songs.Select(s => s.TrackNumber).ToList();
				if (trackNumbers.Any(n => n != null))
				{
					if (trackNumbers.Any(n => n == null) || trackNumbers.First() != 1 || trackNumbers.Last() != trackNumbers.Count)
					{
						inconsistencyRegistrator.RegisterInconsistency_BadTrackNumbersForDisc(disc, trackNumbers);
					}
				}

				//	Checking that all disc songs has equal genre
				var genres = disc.Songs.Select(s => s.Genre).Distinct().ToList();
				if (genres.Count > 1)
				{
					inconsistencyRegistrator.RegisterInconsistency_DifferentGenresForDisc(disc, genres);
				}
			}

			await Task.FromResult(0);
		}
	}
}
