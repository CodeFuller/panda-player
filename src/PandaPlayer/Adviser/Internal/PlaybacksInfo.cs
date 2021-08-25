using System;
using System.Collections.Generic;
using System.Linq;
using PandaPlayer.Adviser.Grouping;
using PandaPlayer.Core.Models;

namespace PandaPlayer.Adviser.Internal
{
	internal class PlaybacksInfo
	{
		private readonly Dictionary<string, int> adviseSetsPlaybacksInfo;

		private readonly Dictionary<ItemId, int> songPlaybacksInfo;

		public PlaybacksInfo(IEnumerable<AdviseSetContent> adviseSets)
		{
			var adviseSetList = adviseSets.ToList();

			adviseSetsPlaybacksInfo = FillDiscPlaybacksInfo(adviseSetList);
			songPlaybacksInfo = FillSongPlaybacksInfo(adviseSetList);
		}

		private static Dictionary<string, int> FillDiscPlaybacksInfo(IEnumerable<AdviseSetContent> adviseSets)
		{
			var playbacksInfo = new Dictionary<string, int>();

			var pairs = adviseSets.Select(x => new
				{
					AdviseSet = x,
					x.LastPlaybackTime,
				})
				.OrderByDescending(x => x.LastPlaybackTime);

			var currentPlaybacksPassed = 0;
			foreach (var pair in pairs)
			{
				var playbacksPassed = pair.LastPlaybackTime != null ? currentPlaybacksPassed++ : Int32.MaxValue;
				playbacksInfo.Add(pair.AdviseSet.Id, playbacksPassed);
			}

			return playbacksInfo;
		}

		private static Dictionary<ItemId, int> FillSongPlaybacksInfo(IEnumerable<AdviseSetContent> adviseSets)
		{
			var playbacksInfo = new Dictionary<ItemId, int>();

			var songs = adviseSets
				.SelectMany(adviseSet => adviseSet.Discs)
				.SelectMany(disc => disc.AllSongs)
				.OrderByDescending(song => song.LastPlaybackTime);

			var currentPlaybacksPassed = 0;
			foreach (var song in songs)
			{
				var playbacksPassed = song.LastPlaybackTime.HasValue ? currentPlaybacksPassed++ : Int32.MaxValue;
				playbacksInfo.Add(song.Id, playbacksPassed);
			}

			return playbacksInfo;
		}

		public int GetPlaybacksPassed(AdviseSetContent adviseSetContent)
		{
			if (adviseSetsPlaybacksInfo.TryGetValue(adviseSetContent.Id, out var playbacksPassed))
			{
				return playbacksPassed;
			}

			throw new InvalidOperationException($"The number of passed playbacks for advise set {adviseSetContent.Id} is unknown");
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
