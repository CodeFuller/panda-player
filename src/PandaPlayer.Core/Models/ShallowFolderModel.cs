using System;
using System.ComponentModel;
using PandaPlayer.Core.Extensions;

namespace PandaPlayer.Core.Models
{
	public class ShallowFolderModel : INotifyPropertyChanged
	{
		public ItemId Id { get; set; }

		public ItemId ParentFolderId { get; set; }

		private string name;

		public string Name
		{
			get => name;
			set => this.SetField(PropertyChanged, ref name, value);
		}

		private AdviseGroupModel adviseGroup;

		public AdviseGroupModel AdviseGroup
		{
			get => adviseGroup;
			set => this.SetField(PropertyChanged, ref adviseGroup, value);
		}

		public DateTimeOffset? DeleteDate { get; set; }

		public bool IsDeleted => DeleteDate != null;

		public bool IsRoot => ParentFolderId == null;

		public event PropertyChangedEventHandler PropertyChanged;

		public ShallowFolderModel CloneShallow()
		{
			return new()
			{
				Id = Id,
				ParentFolderId = ParentFolderId,
				Name = Name,
				AdviseGroup = AdviseGroup,
				DeleteDate = DeleteDate,
			};
		}
	}
}
