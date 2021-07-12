using PandaPlayer.Adviser.Internal;
using PandaPlayer.Core.Models;

namespace PandaPlayer.Adviser.Extensions
{
	internal static class SongModelExtensions
	{
		public static RatingModel GetRatingOrDefault(this SongModel song)
		{
			return song.Rating ?? RatingConstants.ImpliedRatingForNotRatedSongs;
		}
	}
}
