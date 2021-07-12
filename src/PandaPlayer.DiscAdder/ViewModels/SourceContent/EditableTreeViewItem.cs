using GalaSoft.MvvmLight;

namespace PandaPlayer.DiscAdder.ViewModels.SourceContent
{
	internal class EditableTreeViewItem : ViewModelBase
	{
		private bool isSelected;

		public bool IsSelected
		{
			get => isSelected;
			set => Set(ref isSelected, value);
		}

		private bool isExpanded;

		public bool IsExpanded
		{
			get => isExpanded;
			set => Set(ref isExpanded, value);
		}
	}
}
