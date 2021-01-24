using System;
using System.Collections.Generic;
using System.Linq;
using MusicLibrary.DiscAdder.ParsingSong;

namespace MusicLibrary.DiscAdder.ParsingContent
{
	internal class ReferenceDiscParser : IReferenceDiscParser
	{
		private readonly IReferenceSongParser referenceSongParser;

		public ReferenceDiscParser(IReferenceSongParser referenceSongParser)
		{
			this.referenceSongParser = referenceSongParser ?? throw new ArgumentNullException(nameof(referenceSongParser));
		}

		public DiscContent Parse(IEnumerable<string> discContent)
		{
			var content = discContent.ToList();

			// Disc content should contain at least disc directory
			if (content.Count < 1)
			{
				throw new InvalidOperationException($"Invalid reference disc content: {String.Join("\n", content)}");
			}

			var songs = content.Skip(1).Select(s => referenceSongParser.ParseSongTitle(s));
			return new DiscContent(content.First(), songs);
		}
	}
}
