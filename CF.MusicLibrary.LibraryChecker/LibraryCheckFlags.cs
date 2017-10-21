using System;

namespace CF.MusicLibrary.LibraryChecker
{
	[Flags]
	internal enum LibraryCheckFlags
	{
		None = 0,
		CheckDiscsConsistency = 0x01,
		CheckLibraryStorage = 0x02,
		CheckChecksums = 0x04,
		CheckTagData = 0x08,
		CheckImages = 0x10,
		CheckArtistsOnLastFM = 0x20,
		CheckAlbumsOnLastFM = 0x40,
		CheckSongsOnLastFM = 0x80,
	}
}
