using System.Collections.Generic;
using MusicLibrary.Core.Models;

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
