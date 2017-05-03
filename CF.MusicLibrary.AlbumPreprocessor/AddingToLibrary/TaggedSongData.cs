using System;

namespace CF.MusicLibrary.AlbumPreprocessor.AddingToLibrary
{
	public class TaggedSongData
	{
		public string SourceFileName { get; set; }

		public Uri StorageUri { get; set; }

		public string Artist { get; set; }

		public string Album { get; set; }

		public int? Year { get; set; }

		public string Genre { get; set; }

		public int? Track { get; set; }

		public string Title { get; set; }
	}
}
