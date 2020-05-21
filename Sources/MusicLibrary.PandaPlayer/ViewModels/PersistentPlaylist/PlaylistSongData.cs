using MusicLibrary.Logic.Models;

namespace MusicLibrary.PandaPlayer.ViewModels.PersistentPlaylist
{
	public class PlaylistSongData
	{
		public string Id { get; set; }

		public PlaylistSongData()
		{
		}

		public PlaylistSongData(SongModel song)
		{
			Id = song.Id.Value;
		}

		public bool Matches(SongModel song)
		{
			return Id == song.Id.Value;
		}
	}
}
