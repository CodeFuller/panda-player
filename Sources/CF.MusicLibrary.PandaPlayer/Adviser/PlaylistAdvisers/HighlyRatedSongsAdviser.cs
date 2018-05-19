using System;
using System.Collections.Generic;
using System.Linq;
using CF.Library.Core.Facades;
using CF.MusicLibrary.Core.Objects;
using Microsoft.Extensions.Options;

namespace CF.MusicLibrary.PandaPlayer.Adviser.PlaylistAdvisers
{
	public class HighlyRatedSongsAdviser : IPlaylistAdviser
	{
		private readonly Dictionary<Rating, TimeSpan> highlyRatedSongsMaxUnlistenedTerms;

		private readonly IAdviseFactorsProvider adviseFactorsProvider;
		private readonly IClock dateTimeFacade;
		private readonly HighlyRatedSongsAdviserSettings settings;

		public HighlyRatedSongsAdviser(IAdviseFactorsProvider adviseFactorsProvider, IClock dateTimeFacade, IOptions<HighlyRatedSongsAdviserSettings> options)
		{
			this.adviseFactorsProvider = adviseFactorsProvider ?? throw new ArgumentNullException(nameof(adviseFactorsProvider));
			this.dateTimeFacade = dateTimeFacade ?? throw new ArgumentNullException(nameof(dateTimeFacade));
			this.settings = options?.Value ?? throw new ArgumentNullException(nameof(options));

			highlyRatedSongsMaxUnlistenedTerms = settings.MaxUnlistenedTerms
				.ToDictionary(t => t.Rating, t => TimeSpan.FromDays(t.Days));
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

			for (var i = 0; i + settings.OneAdviseSongsNumber <= songs.Count; i += settings.OneAdviseSongsNumber)
			{
				yield return AdvisedPlaylist.ForHighlyRatedSongs(songs.Skip(i).Take(settings.OneAdviseSongsNumber));
			}
		}

		private bool IsTimeToListenHighlyRatedSong(Song song)
		{
			if (!highlyRatedSongsMaxUnlistenedTerms.TryGetValue(song.SafeRating, out var maxUnlistenedTerm))
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
