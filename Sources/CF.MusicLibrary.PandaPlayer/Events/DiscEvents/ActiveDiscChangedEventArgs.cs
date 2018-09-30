using CF.MusicLibrary.Core.Objects;

namespace CF.MusicLibrary.PandaPlayer.Events.DiscEvents
{
	public class ActiveDiscChangedEventArgs : BaseDiscEventArgs
	{
		public ActiveDiscChangedEventArgs(Disc disc)
			: base(disc)
		{
		}
	}
}
