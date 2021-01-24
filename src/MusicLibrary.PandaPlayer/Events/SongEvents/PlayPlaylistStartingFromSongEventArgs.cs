using MusicLibrary.Core.Models;

namespace MusicLibrary.PandaPlayer.Events.SongEvents
{
	public class PlayPlaylistStartingFromSongEventArgs : BaseSongEventArgs
	{
		public PlayPlaylistStartingFromSongEventArgs(SongModel song)
			: base(song)
		{
		}
	}
}
