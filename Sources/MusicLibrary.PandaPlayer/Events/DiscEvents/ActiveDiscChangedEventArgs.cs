using MusicLibrary.Core.Objects;

namespace MusicLibrary.PandaPlayer.Events.DiscEvents
{
	public class ActiveDiscChangedEventArgs : BaseDiscEventArgs
	{
		public ActiveDiscChangedEventArgs(Disc disc)
			: base(disc)
		{
		}
	}
}
