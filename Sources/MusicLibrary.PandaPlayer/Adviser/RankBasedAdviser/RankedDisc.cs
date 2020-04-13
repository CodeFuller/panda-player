using System.Linq;
using MusicLibrary.Core.Objects;

namespace MusicLibrary.PandaPlayer.Adviser.RankBasedAdviser
{
	internal class RankedDisc
	{
		public Disc Disc { get; }

		public int PlaybacksPassed => Disc.PlaybacksPassed.Value;

		public double Rating => Disc.Songs.Select(s => (double)s.SafeRating).Average();

		public RankedDisc(Disc disc)
		{
			Disc = disc;
		}
	}
}
