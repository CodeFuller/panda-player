using System.Collections.Generic;
using GalaSoft.MvvmLight;

namespace PandaPlayer.DiscAdder.ViewModels.SourceContent
{
	internal abstract class BasicDiscTreeViewItem : ViewModelBase
	{
		public abstract IReadOnlyCollection<BasicDiscTreeViewItem> ChildItems { get; }

		public abstract string Title { get; set; }

		private bool isSelected;

		public bool IsSelected
		{
			get => isSelected;
			set => Set(ref isSelected, value);
		}
	}
}
