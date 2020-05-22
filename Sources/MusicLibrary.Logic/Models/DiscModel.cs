using System;
using System.Collections.Generic;

namespace MusicLibrary.Logic.Models
{
	public class DiscModel
	{
		public ItemId Id { get; set; }

		public int? Year { get; set; }

		public string Title { get; set; }

		public string TreeTitle { get; set; }

		public string AlbumTitle { get; set; }

		public IReadOnlyCollection<SongModel> Songs { get; set; }

		public Uri CoverImageUri { get; set; }
	}
}
