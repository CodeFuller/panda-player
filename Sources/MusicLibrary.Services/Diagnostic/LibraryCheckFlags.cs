using System;

namespace MusicLibrary.Services.Diagnostic
{
	[Flags]
	public enum LibraryCheckFlags
	{
		None = 0,
		CheckDiscsConsistency = 0x01,
		CheckStorageConsistency = 0x02,
		CheckContentConsistency = 0x04,
	}
}
