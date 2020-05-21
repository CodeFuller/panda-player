using MusicLibrary.PandaPlayer.ViewModels.Interfaces;

namespace MusicLibrary.PandaPlayer.ViewModels
{
	public class NavigatedViewModelHolder : INavigatedViewModelHolder
	{
		public IEditDiscPropertiesViewModel EditDiscPropertiesViewModel { get; }

		public IEditSongPropertiesViewModel EditSongPropertiesViewModel { get; }

		public IRateSongsViewModel RateSongsViewModel { get; }

		public IEditDiscImageViewModel EditDiscImageViewModel { get; }

		public ILibraryStatisticsViewModel LibraryStatisticsViewModel { get; }

		public NavigatedViewModelHolder(IEditDiscPropertiesViewModel editDiscPropertiesViewModel, IEditSongPropertiesViewModel editSongPropertiesViewModel,
			IRateSongsViewModel rateSongsViewModel, IEditDiscImageViewModel editDiscImageViewModel, ILibraryStatisticsViewModel libraryStatisticsViewModel)
		{
			EditDiscPropertiesViewModel = editDiscPropertiesViewModel;
			EditSongPropertiesViewModel = editSongPropertiesViewModel;
			RateSongsViewModel = rateSongsViewModel;
			EditDiscImageViewModel = editDiscImageViewModel;
			LibraryStatisticsViewModel = libraryStatisticsViewModel;
		}
	}
}
