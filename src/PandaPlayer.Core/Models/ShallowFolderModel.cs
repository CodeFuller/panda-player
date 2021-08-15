using System;

namespace PandaPlayer.Core.Models
{
	public class ShallowFolderModel
	{
		public ItemId Id { get; set; }

		public ItemId ParentFolderId { get; set; }

		public string Name { get; set; }

		public AdviseGroupModel AdviseGroup { get; set; }

		public DateTimeOffset? DeleteDate { get; set; }

		public bool IsDeleted => DeleteDate != null;

		public bool IsRoot => ParentFolderId == null;
	}
}
