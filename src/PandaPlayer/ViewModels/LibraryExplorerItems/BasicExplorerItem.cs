using GalaSoft.MvvmLight;

namespace PandaPlayer.ViewModels.LibraryExplorerItems
{
	public abstract class BasicExplorerItem : ViewModelBase
	{
		public abstract string Title { get; }
	}
}
