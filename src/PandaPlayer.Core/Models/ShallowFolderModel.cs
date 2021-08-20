using System;
using System.ComponentModel;
using PandaPlayer.Core.Extensions;

namespace PandaPlayer.Core.Models
{
	public class ShallowFolderModel : INotifyPropertyChanged
	{
		public ItemId Id { get; set; }

		public ItemId ParentFolderId { get; set; }

		public string Name { get; set; }

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
	}
}
