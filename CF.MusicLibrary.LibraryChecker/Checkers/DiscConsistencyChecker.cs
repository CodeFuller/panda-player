using System;
using System.Collections.Generic;
using System.Linq;
using CF.MusicLibrary.BL.Interfaces;
using CF.MusicLibrary.BL.Objects;
using CF.MusicLibrary.LibraryChecker.Registrators;
using static CF.Library.Core.Application;

namespace CF.MusicLibrary.LibraryChecker.Checkers
{
	public class DiscConsistencyChecker : IDiscConsistencyChecker
	{
		private readonly IMusicStorage musicStorage;
		private readonly ILibraryInconsistencyRegistrator inconsistencyRegistrator;

		public DiscConsistencyChecker(IMusicStorage musicStorage, ILibraryInconsistencyRegistrator inconsistencyRegistrator)
		{
			if (musicStorage == null)
			{
				throw new ArgumentNullException(nameof(musicStorage));
			}
			if (inconsistencyRegistrator == null)
			{
				throw new ArgumentNullException(nameof(inconsistencyRegistrator));
			}

			this.musicStorage = musicStorage;
			this.inconsistencyRegistrator = inconsistencyRegistrator;
		}

		public void CheckDiscsConsistency(IEnumerable<Disc> discs)
		{
			Logger.WriteInfo("Checking discs consistency ...");

			foreach (var disc in discs)
			{
				//	Checking album title
				if (AlbumTitleChecker.AlbumTitleIsSuspicious(disc.AlbumTitle))
				{
					inconsistencyRegistrator.RegisterInconsistency_SuspiciousAlbumTitle(disc);
				}

				//	Check that disc has some songs
				if (!disc.Songs.Any())
				{
					inconsistencyRegistrator.RegisterInconsistency_DiscWithoutSongs(disc);
					continue;
				}

				//	Checking that all song files exist
				foreach (var song in disc.Songs)
				{
					if (!musicStorage.CheckSongContent(song.Uri))
					{
						inconsistencyRegistrator.RegisterInconsistency_BadSongContent(song);
					}
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
		}
	}
}
