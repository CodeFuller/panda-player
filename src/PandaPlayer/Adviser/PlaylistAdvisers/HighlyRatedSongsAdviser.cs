using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using PandaPlayer.Adviser.Extensions;
using PandaPlayer.Adviser.Grouping;
using PandaPlayer.Adviser.Interfaces;
using PandaPlayer.Adviser.Internal;
using PandaPlayer.Adviser.Settings;
using PandaPlayer.Core.Facades;
using PandaPlayer.Core.Models;

namespace PandaPlayer.Adviser.PlaylistAdvisers
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

		public Task<IReadOnlyCollection<AdvisedPlaylist>> Advise(IEnumerable<AdviseGroupContent> adviseGroups, PlaybacksInfo playbacksInfo, CancellationToken cancellationToken)
		{
			var songsToAdvise = adviseGroups
				.SelectMany(adviseGroup => adviseGroup.AdviseSets)
				.SelectMany(adviseSet => adviseSet.Discs)
				.SelectMany(disc => disc.ActiveSongs)
				.Where(IsTimeToListenHighlyRatedSong)
				.OrderByDescending(song => adviseRankCalculator.CalculateSongRank(song, playbacksInfo))
				.ThenByDescending(song => song.GetRatingOrDefault())
				.ToList();

			var playlists = SplitSongsToPlaylists(songsToAdvise).ToList();
			return Task.FromResult<IReadOnlyCollection<AdvisedPlaylist>>(playlists);
		}

		private IEnumerable<AdvisedPlaylist> SplitSongsToPlaylists(IReadOnlyCollection<SongModel> songs)
		{
			for (var i = 0; i + settings.OneAdviseSongsNumber <= songs.Count; i += settings.OneAdviseSongsNumber)
			{
				yield return AdvisedPlaylist.ForHighlyRatedSongs(songs.Skip(i).Take(settings.OneAdviseSongsNumber));
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
