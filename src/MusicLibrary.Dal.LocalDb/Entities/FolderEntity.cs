using System;
using System.Collections.Generic;

namespace MusicLibrary.Dal.LocalDb.Entities
{
	internal class FolderEntity
	{
		public int Id { get; set; }

		public string Name { get; set; }

		public int? ParentFolderId { get; set; }

		public FolderEntity ParentFolder { get; set; }

		public IReadOnlyCollection<FolderEntity> Subfolders { get; set; }

		public IReadOnlyCollection<DiscEntity> Discs { get; set; }

		public DateTimeOffset? DeleteDate { get; set; }
	}
}
