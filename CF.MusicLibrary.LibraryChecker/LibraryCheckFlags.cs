using System;

namespace CF.MusicLibrary.LibraryChecker
{
	[Flags]
	internal enum LibraryCheckFlags
	{
		None = 0,
		CheckDiscsConsistency = 0x01,
		CheckLibraryStorage = 0x02,
		CheckTagData = 0x04,
		CheckDiscArts = 0x08,
		CheckArtistsOnLastFM = 0x10,
		CheckAlbumsOnLastFM = 0x20,
		CheckSongsOnLastFM = 0x40,
	}
}
