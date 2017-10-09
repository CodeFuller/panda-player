using CF.MusicLibrary.BL.Objects;

namespace CF.MusicLibrary.PandaPlayer.Events.DiscEvents
{
	public class DiscArtChangedEventArgs : BaseDiscEventArgs
	{
		public DiscArtChangedEventArgs(Disc disc)
			: base(disc)
		{
		}
	}
}
