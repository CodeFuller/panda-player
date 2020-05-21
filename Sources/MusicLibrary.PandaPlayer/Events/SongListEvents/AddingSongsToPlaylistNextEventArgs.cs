using System.Collections.Generic;
using MusicLibrary.Logic.Models;

namespace MusicLibrary.PandaPlayer.Events.SongListEvents
{
	public class AddingSongsToPlaylistNextEventArgs : AddingSongsToPlaylistEventArgs
	{
		public AddingSongsToPlaylistNextEventArgs(IEnumerable<SongModel> songs)
			: base(songs)
		{
		}
	}
}
