using System.Collections.Generic;
using GalaSoft.MvvmLight;

namespace PandaPlayer.DiscAdder.ViewModels.SourceContent
{
	internal abstract class ActualBasicContentTreeItem : ViewModelBase
	{
		public abstract IEnumerable<ActualBasicContentTreeItem> ChildItems { get; }

		public abstract string ViewTitle { get; set; }

		public abstract bool IsEditable { get; }

		private bool isSelected;

		public bool IsSelected
		{
			get => isSelected;
			set => Set(ref isSelected, value);
		}
	}
}
