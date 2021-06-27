﻿using MusicLibrary.Core.Models;

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
	}
}