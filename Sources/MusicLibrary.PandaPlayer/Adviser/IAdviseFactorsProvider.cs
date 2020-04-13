using MusicLibrary.Core.Objects;

namespace MusicLibrary.PandaPlayer.Adviser
{
	public interface IAdviseFactorsProvider
	{
		double GetFactorForGroupDiscsNumber(int discsNumber);

		double GetFactorForRating(Rating rating);

		double GetFactorForAverageRating(double rating);

		double GetFactorForPlaybackAge(int playbackAge);
	}
}
