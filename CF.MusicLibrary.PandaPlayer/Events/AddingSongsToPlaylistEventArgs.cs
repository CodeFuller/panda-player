using System;
using System.Collections.Generic;
using CF.Library.Core.Extensions;
using CF.MusicLibrary.BL.Objects;

namespace CF.MusicLibrary.PandaPlayer.Events
{
	public abstract class AddingSongsToPlaylistEventArgs : EventArgs
	{
		public IReadOnlyCollection<Song> Songs { get; }

		protected AddingSongsToPlaylistEventArgs(IEnumerable<Song> songs)
		{
			Songs = songs.ToCollection();
		}
	}
}
