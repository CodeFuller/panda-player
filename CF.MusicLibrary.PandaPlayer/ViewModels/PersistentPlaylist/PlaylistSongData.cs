using System;
using CF.MusicLibrary.Core.Objects;

namespace CF.MusicLibrary.PandaPlayer.ViewModels.PersistentPlaylist
{
	public class PlaylistSongData
	{
		public int Id { get; set; }

		public Uri Uri { get; set; }

		public PlaylistSongData()
		{
		}

		public PlaylistSongData(Song song)
		{
			Id = song.Id;
			Uri = song.Uri;
		}

		public bool Matches(Song song)
		{
			return Id == song.Id && Uri == song.Uri;
		}
	}
}
