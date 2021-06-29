using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using MusicLibrary.Core.Facades;
using MusicLibrary.Core.Models;
using MusicLibrary.PandaPlayer.Adviser.Extensions;
using MusicLibrary.PandaPlayer.Adviser.Interfaces;
using MusicLibrary.PandaPlayer.Adviser.Internal;
using MusicLibrary.PandaPlayer.Adviser.Settings;

namespace MusicLibrary.PandaPlayer.Adviser.PlaylistAdvisers
{
	internal class HighlyRatedSongsAdviser : IPlaylistAdviser
	{
		private readonly Dictionary<RatingModel, TimeSpan> maxTermsForRatings;

		private readonly IAdviseRankCalculator adviseRankCalculator;
		private readonly IClock dateTimeFacade;
		private readonly HighlyRatedSongsAdviserSettings settings;

		public HighlyRatedSongsAdviser(IAdviseRankCalculator adviseRankCalculator, IClock dateTimeFacade, IOptions<HighlyRatedSongsAdviserSettings> options)
		{
			this.adviseRankCalculator = adviseRankCalculator ?? throw new ArgumentNullException(nameof(adviseRankCalculator));
			this.dateTimeFacade = dateTimeFacade ?? throw new ArgumentNullException(nameof(dateTimeFacade));
			this.settings = options?.Value ?? throw new ArgumentNullException(nameof(options));

			if (settings.OneAdviseSongsNumber <= 0)
			{
				throw new InvalidOperationException($"{nameof(settings.OneAdviseSongsNumber)} is not set for highly rated songs adviser");
			}

			maxTermsForRatings = settings.MaxTerms
				.ToDictionary(t => t.Rating, t => TimeSpan.FromDays(t.Days));
		}

		public IEnumerable<AdvisedPlaylist> Advise(IEnumerable<DiscModel> discs, PlaybacksInfo playbacksInfo)
		{
			var songsToAdvise = discs
				.SelectMany(disc => disc.ActiveSongs)
				.Where(IsTimeToListenHighlyRatedSong)
				.OrderByDescending(song => adviseRankCalculator.CalculateSongRank(song, playbacksInfo))
				.ThenByDescending(song => song.GetRatingOrDefault())
				.ToList();

			for (var i = 0; i + settings.OneAdviseSongsNumber <= songsToAdvise.Count; i += settings.OneAdviseSongsNumber)
			{
				yield return AdvisedPlaylist.ForHighlyRatedSongs(songsToAdvise.Skip(i).Take(settings.OneAdviseSongsNumber));
			}
		}

		private bool IsTimeToListenHighlyRatedSong(SongModel song)
		{
			if (!maxTermsForRatings.TryGetValue(song.GetRatingOrDefault(), out var maxTerm))
			{
				return false;
			}

			if (song.LastPlaybackTime == null)
			{
				return true;
			}

			return dateTimeFacade.Now - song.LastPlaybackTime.Value >= maxTerm;
		}
	}
}
