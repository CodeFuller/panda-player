using System;

namespace CF.MusicLibrary.BL
{
	[Flags]
	public enum UpdatedSongProperties
	{
		None,
		Rating = 0x001,
		Artist = 0x002,
		Album = 0x004,
		Year = 0x008,
		Genre = 0x010,
		Track = 0x020,
		Title = 0x040,
	}
}
