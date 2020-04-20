using GalaSoft.MvvmLight;

namespace MusicLibrary.PandaPlayer.ViewModels.LibraryBrowser
{
	public abstract class BasicExplorerItem : ViewModelBase
	{
		public abstract string Title { get; }
	}
}
