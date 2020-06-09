using System;
using System.Collections.Generic;
using System.Linq;
using CF.Library.Core.Exceptions;
using MusicLibrary.DiscAdder.ParsingSong;

namespace MusicLibrary.DiscAdder.ParsingContent
{
	public class EthalonDiscParser : IEthalonDiscParser
	{
		private readonly IEthalonSongParser ethalonSongParser;

		public EthalonDiscParser(IEthalonSongParser ethalonSongParser)
		{
			this.ethalonSongParser = ethalonSongParser ?? throw new ArgumentNullException(nameof(ethalonSongParser));
		}

		public DiscContent Parse(IEnumerable<string> discContent)
		{
			var content = discContent.ToList();

			// Disc content should contain at least disc directory
			if (content.Count < 1)
			{
				throw new InvalidInputDataException("Invalid ethalon disc content", String.Join("\n", content));
			}

			var songs = content.Skip(1).Select(s => ethalonSongParser.ParseSongTitle(s));
			return new DiscContent(content.First(), songs);
		}
	}
}
