using System;
using System.Collections.Generic;
using System.Linq;
using CF.Library.Core.Extensions;
using MusicLibrary.Logic.Models;

namespace MusicLibrary.PandaPlayer.Events.SongListEvents
{
	public abstract class BaseSongListEventArgs : EventArgs
	{
		public IReadOnlyCollection<SongModel> Songs { get; }

		public ItemId DiscId => Songs.Select(s => s.DiscId).UniqueOrDefault();

		protected BaseSongListEventArgs(IEnumerable<SongModel> songs)
		{
			Songs = songs?.ToList() ?? throw new ArgumentNullException(nameof(songs));
		}
	}
}
