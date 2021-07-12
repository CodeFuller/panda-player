using System.Collections.Generic;
using System.Linq;

namespace PandaPlayer.DiscAdder.MusicStorage
{
	internal class AddedDiscInfo
	{
		public short? Year { get; set; }

		public string DiscTitle { get; set; }

		public string TreeTitle { get; set; }

		public string AlbumTitle { get; set; }

		public string Artist { get; set; }

		public string SourcePath { get; set; }

		public IReadOnlyCollection<string> DestinationFolderPath { get; set; }

		public IReadOnlyCollection<AddedSongInfo> Songs { get; }

		public bool HasVariousArtists => Songs.Any(s => s.Artist != null);

		public AddedDiscInfo(IEnumerable<AddedSongInfo> songs)
		{
			Songs = songs.ToList();
		}
	}
}
