using System;
using System.Collections.Generic;
using System.Linq;
using CF.Library.Core.Facades;
using Microsoft.Extensions.Options;
using MusicLibrary.Core.Models;
using MusicLibrary.PandaPlayer.Adviser.Extensions;
using MusicLibrary.PandaPlayer.Adviser.Interfaces;
using MusicLibrary.PandaPlayer.Adviser.Internal;
using MusicLibrary.PandaPlayer.Adviser.Settings;

namespace MusicLibrary.PandaPlayer.Adviser.PlaylistAdvisers
{
	internal class HighlyRatedSongsAdviser : IPlaylistAdviser
	{
		private readonly Dictionary<RatingModel, TimeSpan> highlyRatedSongsMaxUnlistenedTerms;

		private readonly IAdviseFactorsProvider adviseFactorsProvider;
		private readonly IClock dateTimeFacade;
		private readonly HighlyRatedSongsAdviserSettings settings;

		public HighlyRatedSongsAdviser(IAdviseFactorsProvider adviseFactorsProvider, IClock dateTimeFacade, IOptions<HighlyRatedSongsAdviserSettings> options)
		{
			this.adviseFactorsProvider = adviseFactorsProvider ?? throw new ArgumentNullException(nameof(adviseFactorsProvider));
			this.dateTimeFacade = dateTimeFacade ?? throw new ArgumentNullException(nameof(dateTimeFacade));
			this.settings = options?.Value ?? throw new ArgumentNullException(nameof(options));

			if (settings.OneAdviseSongsNumber <= 0)
			{
				throw new InvalidOperationException($"{nameof(settings.OneAdviseSongsNumber)} is not set for highly rated songs adviser");
			}

			highlyRatedSongsMaxUnlistenedTerms = settings.MaxUnlistenedTerms
				.ToDictionary(t => t.Rating, t => TimeSpan.FromDays(t.Days));
		}

		public IEnumerable<AdvisedPlaylist> Advise(IEnumerable<DiscModel> discs, PlaybacksInfo playbacksInfo)
		{
			var songsToAdvise = discs
				.SelectMany(d => d.Songs.Where(song => !song.IsDeleted))
				.Where(IsTimeToListenHighlyRatedSong)
				.OrderByDescending(song => GetRankForSong(song, playbacksInfo))
				.ThenByDescending(song => song.GetRatingOrDefault())
				.ToList();

			for (var i = 0; i + settings.OneAdviseSongsNumber <= songsToAdvise.Count; i += settings.OneAdviseSongsNumber)
			{
				yield return AdvisedPlaylist.ForHighlyRatedSongs(songsToAdvise.Skip(i).Take(settings.OneAdviseSongsNumber));
			}
		}

		private double GetRankForSong(SongModel song, PlaybacksInfo playbacksInfo)
		{
			return song.LastPlaybackTime.HasValue ? GetRankForListenedSong(song, playbacksInfo) : Double.MaxValue;
		}

		private double GetRankForListenedSong(SongModel song, PlaybacksInfo playbacksInfo)
		{
			var factorForRating = adviseFactorsProvider.GetFactorForRating(song.GetRatingOrDefault());
			var factorForPlaybacksAge = adviseFactorsProvider.GetFactorForPlaybackAge(playbacksInfo.GetPlaybacksPassed(song));

			return factorForRating * factorForPlaybacksAge;
		}

		private bool IsTimeToListenHighlyRatedSong(SongModel song)
		{
			if (!highlyRatedSongsMaxUnlistenedTerms.TryGetValue(song.GetRatingOrDefault(), out var maxUnlistenedTerm))
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
