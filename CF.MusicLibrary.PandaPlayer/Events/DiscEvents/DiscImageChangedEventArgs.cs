using CF.MusicLibrary.Core.Objects;

namespace CF.MusicLibrary.PandaPlayer.Events.DiscEvents
{
	public class DiscImageChangedEventArgs : BaseDiscEventArgs
	{
		public DiscImageChangedEventArgs(Disc disc)
			: base(disc)
		{
		}
	}
}
