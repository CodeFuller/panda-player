using System;

namespace CF.MusicLibrary.LibraryChecker
{
	[Flags]
	public enum LibraryCheckFlags
	{
		None = 0,
		CheckDiscsConsistency = 0x01,
		CheckLibraryStorage = 0x02,
		CheckChecksums = 0x04,
		CheckTagData = 0x08,
		CheckImages = 0x10,
		CheckArtistsOnLastFm = 0x20,
		CheckAlbumsOnLastFm = 0x40,
		CheckSongsOnLastFm = 0x80,
	}
}
