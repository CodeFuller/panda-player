using System;

namespace CF.MusicLibrary.LibraryChecker
{
	[Flags]
	internal enum LaunchCommandFlags
	{
		None = 0,
		Check = 0x01,
		UnifyTags = 0x02,
	}
}
