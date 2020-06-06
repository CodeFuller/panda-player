using GalaSoft.MvvmLight;

namespace MusicLibrary.PandaPlayer.ViewModels.LibraryExplorerItems
{
	public abstract class BasicExplorerItem : ViewModelBase
	{
		public abstract string Title { get; }
	}
}
