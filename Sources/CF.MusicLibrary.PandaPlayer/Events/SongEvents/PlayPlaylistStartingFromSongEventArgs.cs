using CF.MusicLibrary.Core.Objects;

namespace CF.MusicLibrary.PandaPlayer.Events.SongEvents
{
	public class PlayPlaylistStartingFromSongEventArgs : BaseSongEventArgs
	{
		public PlayPlaylistStartingFromSongEventArgs(Song song)
			: base(song)
		{
		}
	}
}
