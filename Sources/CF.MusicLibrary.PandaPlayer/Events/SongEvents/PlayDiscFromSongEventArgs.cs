using CF.MusicLibrary.Core.Objects;

namespace CF.MusicLibrary.PandaPlayer.Events.SongEvents
{
	internal class PlayDiscFromSongEventArgs : BaseSongEventArgs
	{
		public PlayDiscFromSongEventArgs(Song song)
			: base(song)
		{
		}
	}
}
