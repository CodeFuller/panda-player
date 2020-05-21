using MusicLibrary.Logic.Models;

namespace MusicLibrary.PandaPlayer.Events.SongEvents
{
	internal class PlayDiscFromSongEventArgs : BaseSongEventArgs
	{
		public PlayDiscFromSongEventArgs(SongModel song)
			: base(song)
		{
		}
	}
}
