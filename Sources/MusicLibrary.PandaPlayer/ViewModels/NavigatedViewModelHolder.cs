using MusicLibrary.PandaPlayer.ViewModels.Interfaces;

namespace MusicLibrary.PandaPlayer.ViewModels
{
	public class NavigatedViewModelHolder : INavigatedViewModelHolder
	{
		public IEditDiscPropertiesViewModel EditDiscPropertiesViewModel { get; }

		public IEditSongPropertiesViewModel EditSongPropertiesViewModel { get; }

		public IRateDiscViewModel RateDiscViewModel { get; }

		public IEditDiscImageViewModel EditDiscImageViewModel { get; }

		public ILibraryStatisticsViewModel LibraryStatisticsViewModel { get; }

		public NavigatedViewModelHolder(IEditDiscPropertiesViewModel editDiscPropertiesViewModel, IEditSongPropertiesViewModel editSongPropertiesViewModel,
			IRateDiscViewModel rateDiscViewModel, IEditDiscImageViewModel editDiscImageViewModel, ILibraryStatisticsViewModel libraryStatisticsViewModel)
		{
			EditDiscPropertiesViewModel = editDiscPropertiesViewModel;
			EditSongPropertiesViewModel = editSongPropertiesViewModel;
			RateDiscViewModel = rateDiscViewModel;
			EditDiscImageViewModel = editDiscImageViewModel;
			LibraryStatisticsViewModel = libraryStatisticsViewModel;
		}
	}
}
