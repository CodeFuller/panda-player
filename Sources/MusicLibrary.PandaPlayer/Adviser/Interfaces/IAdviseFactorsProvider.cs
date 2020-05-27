using MusicLibrary.Core.Models;

namespace MusicLibrary.PandaPlayer.Adviser.Interfaces
{
	internal interface IAdviseFactorsProvider
	{
		double GetFactorForGroupDiscsNumber(int discsNumber);

		double GetFactorForRating(RatingModel rating);

		double GetFactorForAverageRating(double rating);

		double GetFactorForPlaybackAge(int playbackAge);
	}
}
