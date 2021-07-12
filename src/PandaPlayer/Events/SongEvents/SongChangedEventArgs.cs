using System;
using PandaPlayer.Core.Models;

namespace PandaPlayer.Events.SongEvents
{
	internal class SongChangedEventArgs : BaseSongEventArgs
	{
		public string PropertyName { get; }

		public SongChangedEventArgs(SongModel song, string propertyName)
			: base(song)
		{
			PropertyName = propertyName ?? throw new ArgumentNullException(nameof(propertyName));
		}
	}
}
