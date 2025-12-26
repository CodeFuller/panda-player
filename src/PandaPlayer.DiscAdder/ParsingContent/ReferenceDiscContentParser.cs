using System;
using System.Collections.Generic;
using System.Linq;
using PandaPlayer.DiscAdder.ParsingSong;

namespace PandaPlayer.DiscAdder.ParsingContent
{
	internal class ReferenceDiscContentParser : IReferenceDiscContentParser
	{
		private readonly IReferenceSongContentParser referenceSongContentParser;

		public ReferenceDiscContentParser(IReferenceSongContentParser referenceSongContentParser)
		{
			this.referenceSongContentParser = referenceSongContentParser ?? throw new ArgumentNullException(nameof(referenceSongContentParser));
		}

		public ReferenceDiscContent Parse(IEnumerable<string> rawReferenceDiscContent)
		{
			var content = rawReferenceDiscContent.ToList();

			// Disc content should contain at least disc directory.
			if (content.Count == 0)
			{
				throw new InvalidOperationException($"Invalid reference disc content: {String.Join(Environment.NewLine, content)}");
			}

			var expectedSongs = content.Skip(1).Select((x, i) => referenceSongContentParser.Parse(i + 1, x));
			return new ReferenceDiscContent(content.First(), expectedSongs);
		}
	}
}
