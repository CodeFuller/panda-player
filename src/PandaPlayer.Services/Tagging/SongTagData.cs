using PandaPlayer.Core.Models;

namespace PandaPlayer.Services.Tagging
{
	public class SongTagData
	{
		public string Artist { get; set; }

		public string Album { get; set; }

		public int? Year { get; set; }

		public string Genre { get; set; }

		public int? Track { get; set; }

		public string Title { get; set; }

		public SongTagData()
		{
		}

		public SongTagData(SongModel song)
		{
			Artist = song.Artist?.Name;
			Album = song.Disc.AlbumTitle;
			Year = song.Disc.Year;
			Genre = song.Genre?.Name;
			Track = song.TrackNumber;
			Title = song.Title;
		}
	}
}
