using System;
using System.Collections.ObjectModel;

namespace CF.MusicLibrary.BL.Objects
{
	public class Disc
	{
		public int Id { get; set; }

		public string Title { get; set; }

		public Uri Uri { get; set; }

		public Collection<Song> Songs { get; set; } = new Collection<Song>();
	}
}
