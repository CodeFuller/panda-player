using System;
using System.Collections.Generic;
using System.Linq;
using MusicLibrary.Dal.LocalDb.Extensions;
using MusicLibrary.Dal.LocalDb.Internal;

namespace MusicLibrary.Dal.LocalDb.Entities
{
	internal class DiscEntity
	{
		public int Id { get; set; }

		public FolderEntity Folder
		{
			get
			{
				var uriParts = new ItemUriParts(Uri);
				return new FolderEntity
				{
					Id = ItemUriParts.Join(uriParts.Take(uriParts.Count - 1)).ToItemId(),
					Name = uriParts[^2],
				};
			}
		}

		public string Title { get; set; }

		public string AlbumTitle { get; set; }

		public Uri Uri { get; set; }

		public IReadOnlyCollection<SongEntity> Songs { get; set; }

		public IReadOnlyCollection<DiscImageEntity> Images { get; set; }
	}
}
