using System.Collections.Generic;
using MusicLibrary.Logic.Models;

namespace MusicLibrary.PandaPlayer.Events.SongListEvents
{
	public class AddingSongsToPlaylistLastEventArgs : AddingSongsToPlaylistEventArgs
	{
		public AddingSongsToPlaylistLastEventArgs(IEnumerable<SongModel> songs)
			: base(songs)
		{
		}
	}
}
