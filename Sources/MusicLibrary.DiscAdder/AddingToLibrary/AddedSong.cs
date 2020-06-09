using System;
using System.Collections.Generic;
using MusicLibrary.Core.Models;

namespace MusicLibrary.DiscAdder.AddingToLibrary
{
	internal class AddedSong
	{
		public SongModel Song { get; }

		public string SourceFileName { get; }

		// TODO: It is quite strange to pass disc folder path with each song. Can we have AddedDisc with this information filled?
		public IReadOnlyCollection<string> DiscFolderPath { get; }

		public AddedSong(SongModel song, string sourceFileName, IReadOnlyCollection<string> discFolderPath)
		{
			Song = song ?? throw new ArgumentNullException(nameof(song));
			SourceFileName = sourceFileName ?? throw new ArgumentNullException(nameof(sourceFileName));
			DiscFolderPath = discFolderPath ?? throw new ArgumentNullException(nameof(discFolderPath));
		}
	}
}
