using System.Collections.Generic;
using System.Linq;
using CF.MusicLibrary.BL.Objects;
using static CF.Library.Core.Extensions.FormattableStringExtensions;

namespace CF.MusicLibrary.PandaPlayer.DiscAdviser
{
	public class AdvisedPlaylist
	{
		public string Title { get; }

		public IReadOnlyCollection<Song> Songs { get; }

		public AdvisedPlaylist(Disc disc)
		{
			Title = disc.Artist == null ? disc.Title : Current($"{disc.Artist.Name} - {disc.Title}");
			Songs = new List<Song>(disc.Songs);
		}

		public AdvisedPlaylist(string title, IEnumerable<Song> songs)
		{
			Title = title;
			Songs = songs.ToList();
		}
	}
}
