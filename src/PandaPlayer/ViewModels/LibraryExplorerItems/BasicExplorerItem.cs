using GalaSoft.MvvmLight;
using MaterialDesignThemes.Wpf;

namespace PandaPlayer.ViewModels.LibraryExplorerItems
{
	public abstract class BasicExplorerItem : ViewModelBase
	{
		public abstract string Title { get; }

		public abstract PackIconKind IconKind { get; }

		public abstract bool IsDeleted { get; }
	}
}
