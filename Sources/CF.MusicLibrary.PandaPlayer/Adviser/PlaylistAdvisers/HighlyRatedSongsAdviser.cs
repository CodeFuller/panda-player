using System;
using System.Collections.Generic;
using System.Linq;
using CF.Library.Core.Facades;
using CF.MusicLibrary.Core.Objects;

namespace CF.MusicLibrary.PandaPlayer.Adviser.PlaylistAdvisers
{
	public class HighlyRatedSongsAdviser : IPlaylistAdviser
	{
		public static int OneAdviseSongsNumber => 12;

		private readonly Dictionary<Rating, TimeSpan> highlyRatedSongsMaxUnlistenedTerms;

		private readonly IAdviseFactorsProvider adviseFactorsProvider;
		private readonly IClock dateTimeFacade;

		public HighlyRatedSongsAdviser(IAdviseFactorsProvider adviseFactorsProvider, IClock dateTimeFacade)
		{
			if (adviseFactorsProvider == null)
			{
				throw new ArgumentNullException(nameof(adviseFactorsProvider));
			}
			if (dateTimeFacade == null)
			{
				throw new ArgumentNullException(nameof(dateTimeFacade));
			}

			this.adviseFactorsProvider = adviseFactorsProvider;
			this.dateTimeFacade = dateTimeFacade;

			highlyRatedSongsMaxUnlistenedTerms = new Dictionary<Rating, TimeSpan>();
			highlyRatedSongsMaxUnlistenedTerms.Add(Rating.R10, TimeSpan.FromDays(30));
			highlyRatedSongsMaxUnlistenedTerms.Add(Rating.R9, TimeSpan.FromDays(60));
			highlyRatedSongsMaxUnlistenedTerms.Add(Rating.R8, TimeSpan.FromDays(90));
		}

		public IEnumerable<AdvisedPlaylist> Advise(DiscLibrary discLibrary)
		{
			int playbacksPassed = 0;
			var songs = discLibrary.Songs.OrderByDescending(d => d.LastPlaybackTime)
				.Select(s => new
				{
					Song = s,
					Rating = s.SafeRating,
					Rank = !s.LastPlaybackTime.HasValue ? Double.MaxValue :
						adviseFactorsProvider.GetFactorForRating(s.SafeRating) * adviseFactorsProvider.GetFactorForPlaybackAge(playbacksPassed++),
				})
				.Where(s => IsTimeToListenHighlyRatedSong(s.Song))
				.OrderByDescending(s => s.Rank)
				.ThenByDescending(s => s.Rating)
				.Select(s => s.Song)
				.ToList();

			for (var i = 0; i + OneAdviseSongsNumber <= songs.Count; i += OneAdviseSongsNumber)
			{
				yield return AdvisedPlaylist.ForHighlyRatedSongs(songs.Skip(i).Take(OneAdviseSongsNumber));
			}
		}

		private bool IsTimeToListenHighlyRatedSong(Song song)
		{
			TimeSpan maxUnlistenedTerm;
			if (!highlyRatedSongsMaxUnlistenedTerms.TryGetValue(song.SafeRating, out maxUnlistenedTerm))
			{
				return false;
			}

			if (song.LastPlaybackTime == null)
			{
				return true;
			}

			return dateTimeFacade.Now - song.LastPlaybackTime.Value >= maxUnlistenedTerm;
		}
	}
}
