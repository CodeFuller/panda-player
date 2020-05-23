using System;
using System.Collections.Generic;

namespace MusicLibrary.Dal.LocalDb.Entities
{
	internal class DiscEntity
	{
		public int Id { get; set; }

		public string Title { get; set; }

		public string AlbumTitle { get; set; }

		public Uri Uri { get; set; }

		public IReadOnlyCollection<SongEntity> Songs { get; set; }

		public IReadOnlyCollection<DiscImageEntity> Images { get; set; }
	}
}
