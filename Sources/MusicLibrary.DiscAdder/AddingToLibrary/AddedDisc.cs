using System;
using System.Collections.Generic;
using System.Linq;
using MusicLibrary.Core.Models;

namespace MusicLibrary.DiscAdder.AddingToLibrary
{
	internal class AddedDisc
	{
		public DiscModel Disc { get; }

		public bool IsNewDisc { get; }

		public string SourcePath { get; }

		public IReadOnlyCollection<string> FolderPath { get; }

		public AddedDisc(DiscModel disc, bool isNewDisc, string sourcePath, IEnumerable<string> folderPath)
		{
			Disc = disc ?? throw new ArgumentNullException(nameof(disc));
			IsNewDisc = isNewDisc;
			SourcePath = sourcePath ?? throw new ArgumentNullException(nameof(sourcePath));
			FolderPath = folderPath?.ToList() ?? throw new ArgumentNullException(nameof(folderPath));
		}
	}
}
