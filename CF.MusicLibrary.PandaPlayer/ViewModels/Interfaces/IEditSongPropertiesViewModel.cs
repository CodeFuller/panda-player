using System;
using System.Collections.Generic;
using CF.MusicLibrary.BL.Objects;

namespace CF.MusicLibrary.PandaPlayer.ViewModels.Interfaces
{
	public interface IEditSongPropertiesViewModel
	{
		Uri UpdatedSongUri { get; }

		void Load(IEnumerable<Song> songs);

		IEnumerable<Song> GetUpdatedSongs();
	}
}
