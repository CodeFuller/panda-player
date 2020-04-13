using MusicLibrary.Core.Objects;

namespace MusicLibrary.PandaPlayer.Events.SongEvents
{
	public class PlayPlaylistStartingFromSongEventArgs : BaseSongEventArgs
	{
		public PlayPlaylistStartingFromSongEventArgs(Song song)
			: base(song)
		{
		}
	}
}
