using System;
using System.Collections.Generic;
using System.Linq;
using MusicLibrary.Logic.Models;
using MusicLibrary.PandaPlayer.Extensions;

namespace MusicLibrary.PandaPlayer.Events.SongListEvents
{
	public abstract class BaseSongListEventArgs : EventArgs
	{
		public IReadOnlyCollection<SongModel> Songs { get; }

		public DiscModel Disc => Songs.UniqueOrDefault(s => s.Disc.Id)?.Disc;

		protected BaseSongListEventArgs(IEnumerable<SongModel> songs)
		{
			Songs = songs?.ToList() ?? throw new ArgumentNullException(nameof(songs));
		}
	}
}
