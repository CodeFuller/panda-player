using System.Linq;
using CF.MusicLibrary.BL.Objects;

namespace CF.MusicLibrary.PandaPlayer.DiscAdviser
{
	internal class RankedDisc
	{
		public Disc Disc { get; }

		public int PlaybacksPassed => Disc.PlaybacksPassed.Value;

		public double Rating => Disc.Songs.Select(s => (double) s.SafeRating).Average();

		public RankedDisc(Disc disc)
		{
			Disc = disc;
		}
	}
}
