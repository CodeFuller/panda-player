using GalaSoft.MvvmLight;

namespace CF.MusicLibrary.AlbumPreprocessor.ViewModels
{
	public class EditableTreeViewItem : ViewModelBase
	{
		private bool isSelected;
		public bool IsSelected
		{
			get { return isSelected; }
			set
			{
				Set(ref isSelected, value);
			}
		}

		private bool isExpanded;
		public bool IsExpanded
		{
			get { return isExpanded; }
			set
			{
				Set(ref isExpanded, value);
			}
		}
	}
}
