using System;

namespace MusicLibrary.Services.Media
{
	public class SongMediaInfo
	{
		public int Size { get; set; }

		public int Bitrate { get; set; }

		public TimeSpan Duration { get; set; }
	}
}
