using System;
using System.Linq;
using CF.MusicLibrary.BL.Objects;

namespace CF.MusicLibrary.BL.DiscAdviser
{
	public class RankCalculator : IRankCalculator
	{
		//	Album with rating 3.0 is advised (x RatingFactorMultiplier) more often that album with rating 2.5
		private const double RatingFactorMultiplier = 2;

		public double CalculateArtistRank(LibraryArtist artist)
		{
			if (artist == null)
			{
				throw new ArgumentNullException(nameof(artist));
			}

			var artistDiscs = artist.Discs;

			return CalculateArtistFactorForAlbumsNumber(artistDiscs.Count) *
				   CalculateArtistFactorForAverageAlbumRating(artistDiscs.Select(d => d.Rating).Average()) *
				   CalculateArtistFactorForPlaybackAge(artist.PlaybacksPassed);
		}

		public double CalculateDiscRank(LibraryDisc disc)
		{
			if (disc == null)
			{
				throw new ArgumentNullException(nameof(disc));
			}

			return CalculateDiscFactorForRating(disc) * CalculateDiscFactorForPlaybackAge(disc.PlaybacksPassed);
		}

		private static double CalculateArtistFactorForAlbumsNumber(int albumsNumber)
		{
			return Math.Sqrt(albumsNumber);
		}

		private static double CalculateArtistFactorForAverageAlbumRating(double averageAlbumRating)
		{
			return Math.Pow(RatingFactorMultiplier, averageAlbumRating);
		}

		private static double CalculateArtistFactorForPlaybackAge(int playbackAge)
		{
			return playbackAge;
		}

		private static double CalculateDiscFactorForRating(LibraryDisc disc)
		{
			return Math.Pow(RatingFactorMultiplier, disc.Rating);
		}

		private static double CalculateDiscFactorForPlaybackAge(int playbackAge)
		{
			return playbackAge;
		}
	}
}
