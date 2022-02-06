using System;
using System.Collections.Generic;
using System.Linq;

namespace PandaPlayer.DiscAdder
{
	internal class ReferenceDiscContent
	{
		public string ExpectedDirectoryPath { get; }

		public IReadOnlyCollection<ReferenceSongContent> ExpectedSongs { get; }

		public ReferenceDiscContent(string expectedDirectoryPath, IEnumerable<ReferenceSongContent> expectedSongs)
		{
			ExpectedDirectoryPath = expectedDirectoryPath ?? throw new ArgumentNullException(nameof(expectedDirectoryPath));
			ExpectedSongs = expectedSongs?.ToList() ?? throw new ArgumentNullException(nameof(expectedSongs));
		}
	}
}
