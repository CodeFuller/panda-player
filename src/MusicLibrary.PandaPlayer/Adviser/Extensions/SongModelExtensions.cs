using MusicLibrary.Core.Models;
using MusicLibrary.PandaPlayer.Adviser.Internal;

namespace MusicLibrary.PandaPlayer.Adviser.Extensions
{
	internal static class SongModelExtensions
	{
		public static RatingModel GetRatingOrDefault(this SongModel song)
		{
			return song.Rating ?? RatingConstants.ImpliedRatingForNotRatedSongs;
		}
	}
}
