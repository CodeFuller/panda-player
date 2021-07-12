using System;
using PandaPlayer.Core.Models;

namespace PandaPlayer.DiscAdder.AddedContent
{
	internal class AddedSong
	{
		public AddedDisc Disc { get; }

		public SongModel Song { get; }

		public string SourceFileName { get; }

		public AddedSong(AddedDisc disc, SongModel song, string sourceFileName)
		{
			Disc = disc ?? throw new ArgumentNullException(nameof(disc));
			Song = song ?? throw new ArgumentNullException(nameof(song));
			SourceFileName = sourceFileName ?? throw new ArgumentNullException(nameof(sourceFileName));
		}
	}
}
