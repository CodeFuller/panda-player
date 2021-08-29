using System;
using System.Linq;
using PandaPlayer.Core.Models;

namespace PandaPlayer.Adviser.Extensions
{
	internal static class DiscModelExtensions
	{
		public static DateTimeOffset? GetLastPlaybackTime(this DiscModel disc)
		{
			var analyzedSongs = (disc.IsDeleted ? disc.AllSongs : disc.ActiveSongs).ToList();
			return analyzedSongs.Any(s => s.LastPlaybackTime == null) ? null : analyzedSongs.Select(s => s.LastPlaybackTime).Min();
		}

		public static double GetRating(this DiscModel disc)
		{
			return disc.ActiveSongs
				.Select(song => song.GetRatingOrDefault())
				.Select(rating => rating.GetRatingValueForAdviser())
				.Average();
		}
	}
}
