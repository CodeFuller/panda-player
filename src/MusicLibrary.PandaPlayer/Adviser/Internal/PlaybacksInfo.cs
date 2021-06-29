using System;
using System.Collections.Generic;
using System.Linq;
using MusicLibrary.Core.Models;
using MusicLibrary.PandaPlayer.Adviser.Extensions;

namespace MusicLibrary.PandaPlayer.Adviser.Internal
{
	internal class PlaybacksInfo
	{
		private readonly Dictionary<ItemId, int> discPlaybacksInfo;

		private readonly Dictionary<ItemId, int> songPlaybacksInfo;

		public PlaybacksInfo(IEnumerable<DiscModel> discs)
		{
			var discsList = discs.ToList();

			discPlaybacksInfo = FillDiscPlaybacksInfo(discsList);
			songPlaybacksInfo = FillSongPlaybacksInfo(discsList);
		}

		private static Dictionary<ItemId, int> FillDiscPlaybacksInfo(IEnumerable<DiscModel> discs)
		{
			var playbacksInfo = new Dictionary<ItemId, int>();

			var currentPlaybacksPassed = 0;
			foreach (var disc in discs.OrderByDescending(d => d.GetLastPlaybackTime()))
			{
				var playbacksPassed = disc.GetLastPlaybackTime().HasValue ? currentPlaybacksPassed++ : Int32.MaxValue;
				playbacksInfo.Add(disc.Id, playbacksPassed);
			}

			return playbacksInfo;
		}

		private static Dictionary<ItemId, int> FillSongPlaybacksInfo(IEnumerable<DiscModel> discs)
		{
			var playbacksInfo = new Dictionary<ItemId, int>();

			var currentPlaybacksPassed = 0;
			foreach (var song in discs.SelectMany(disc => disc.AllSongs).OrderByDescending(song => song.LastPlaybackTime))
			{
				var playbacksPassed = song.LastPlaybackTime.HasValue ? currentPlaybacksPassed++ : Int32.MaxValue;
				playbacksInfo.Add(song.Id, playbacksPassed);
			}

			return playbacksInfo;
		}

		public int GetPlaybacksPassed(DiscModel disc)
		{
			if (discPlaybacksInfo.TryGetValue(disc.Id, out var playbacksPassed))
			{
				return playbacksPassed;
			}

			throw new InvalidOperationException($"The number of passed playbacks for disc {disc.Id} is unknown");
		}

		public int GetPlaybacksPassed(SongModel song)
		{
			if (songPlaybacksInfo.TryGetValue(song.Id, out var playbacksPassed))
			{
				return playbacksPassed;
			}

			throw new InvalidOperationException($"The number of passed playbacks for song {song.Id} is unknown");
		}
	}
}
