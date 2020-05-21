using System.Collections.Generic;
using MusicLibrary.Logic.Models;

namespace MusicLibrary.PandaPlayer.Events.SongListEvents
{
	public abstract class AddingSongsToPlaylistEventArgs : BaseSongListEventArgs
	{
		protected AddingSongsToPlaylistEventArgs(IEnumerable<SongModel> songs)
			: base(songs)
		{
		}
	}
}
