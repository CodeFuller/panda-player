using System;
using System.Collections.Generic;
using System.Linq;

namespace PandaPlayer.DiscAdder
{
	internal class ActualDiscContent
	{
		public string DiscDirectory { get; }

		public IReadOnlyCollection<ActualSongContent> Songs { get; }

		public ActualDiscContent(string discDirectory, IEnumerable<ActualSongContent> songs)
		{
			DiscDirectory = discDirectory;
			Songs = songs?.ToList() ?? throw new ArgumentNullException(nameof(songs));
		}
	}
}
