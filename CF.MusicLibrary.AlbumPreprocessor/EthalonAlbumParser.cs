using System;
using System.Collections.Generic;
using System.Linq;
using CF.Library.Core.Exceptions;
using CF.MusicLibrary.AlbumPreprocessor.Interfaces;

namespace CF.MusicLibrary.AlbumPreprocessor
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
			// Album content should contain at least album directory and one song
			if (content.Count < 2)
			{
				throw new InvalidInputDataException("Invalid ethalon album content", String.Join("\n", content));
			}

			var songs = content.Skip(1).Select(s => ethalonSongParser.ParseSongTitle(s));
			return new AlbumContent(content.First(), songs);
		}
	}
}
