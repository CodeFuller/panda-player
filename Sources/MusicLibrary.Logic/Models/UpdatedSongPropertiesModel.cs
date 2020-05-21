using System;

namespace MusicLibrary.Logic.Models
{
	[Flags]
#pragma warning disable CA1714 // Flags enums should have plural names
	public enum UpdatedSongPropertiesModel
#pragma warning restore CA1714 // Flags enums should have plural names
	{
		None,
		Rating = 0x001,
		Artist = 0x002,
		Album = 0x004,
		Year = 0x008,
		Genre = 0x010,
		Track = 0x020,
		Title = 0x040,
		Uri = 0x080,
		ForceTagUpdate = 0x100,
	}
}
