using System;

namespace CF.MusicLibrary.LibraryChecker
{
	[Flags]
	internal enum LibraryCheckFlags
	{
		None = 0,
		CheckDiscsConsistency = 0x01,
		CheckTagData = 0x02,
		CheckArtistsOnLastFM = 0x04,
		CheckAlbumsOnLastFM = 0x08,
		CheckSongsOnLastFM = 0x10,
	}
}
