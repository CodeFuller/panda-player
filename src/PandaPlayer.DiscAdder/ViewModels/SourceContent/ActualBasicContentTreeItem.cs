using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;

namespace PandaPlayer.DiscAdder.ViewModels.SourceContent
{
	internal abstract class ActualBasicContentTreeItem : ObservableObject
	{
		public abstract IEnumerable<ActualBasicContentTreeItem> ChildItems { get; }

		public abstract string ViewTitle { get; set; }

		public abstract bool IsEditable { get; }

		private bool isSelected;

		public bool IsSelected
		{
			get => isSelected;
			set => SetProperty(ref isSelected, value);
		}
	}
}
