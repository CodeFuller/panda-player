﻿using MusicLibrary.Core.Models;

namespace MusicLibrary.PandaPlayer.Events.DiscEvents
{
	public class NavigateLibraryExplorerToDiscEventArgs : BaseDiscEventArgs
	{
		public NavigateLibraryExplorerToDiscEventArgs(DiscModel disc)
			: base(disc)
		{
		}
	}
}