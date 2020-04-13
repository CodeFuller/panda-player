using MusicLibrary.Core.Objects;

namespace MusicLibrary.PandaPlayer.Events.DiscEvents
{
	public class DiscImageChangedEventArgs : BaseDiscEventArgs
	{
		public DiscImageChangedEventArgs(Disc disc)
			: base(disc)
		{
		}
	}
}
