using System;
using CF.MusicLibrary.Tagger;

namespace CF.MusicLibrary.AlbumPreprocessor
{
	public class TaggedSongData : SongTagData
	{
		public string SourceFileName { get; set; }

		public Uri StorageUri { get; set; }
	}
}
