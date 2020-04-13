using MusicLibrary.Core.Objects;

namespace MusicLibrary.PandaPlayer.Events.SongEvents
{
	internal class PlayDiscFromSongEventArgs : BaseSongEventArgs
	{
		public PlayDiscFromSongEventArgs(Song song)
			: base(song)
		{
		}
	}
}
