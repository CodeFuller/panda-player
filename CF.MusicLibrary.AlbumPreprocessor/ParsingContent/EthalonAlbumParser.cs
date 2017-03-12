using System;
using System.Collections.Generic;
using System.Linq;
using CF.Library.Core.Exceptions;
using CF.MusicLibrary.AlbumPreprocessor.ParsingSong;

namespace CF.MusicLibrary.AlbumPreprocessor.ParsingContent
{
	public class EthalonAlbumParser : IEthalonAlbumParser
	{
		private readonly IEthalonSongParser ethalonSongParser;

		public EthalonAlbumParser(IEthalonSongParser ethalonSongParser)
		{
			if (ethalonSongParser == null)
			{
				throw new ArgumentNullException(nameof(ethalonSongParser));
			}

			this.ethalonSongParser = ethalonSongParser;
		}

		public AlbumContent Parse(IEnumerable<string> albumContent)
		{
			var content = albumContent.ToList();
			// Album content should contain at least album directory
			if (content.Count < 1)
			{
				throw new InvalidInputDataException("Invalid ethalon album content", String.Join("\n", content));
			}

			var songs = content.Skip(1).Select(s => ethalonSongParser.ParseSongTitle(s));
			return new AlbumContent(content.First(), songs);
		}
	}
}
