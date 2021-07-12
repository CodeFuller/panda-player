using PandaPlayer.Core.Models;

namespace PandaPlayer.ViewModels.PersistentPlaylist
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
	}
}
