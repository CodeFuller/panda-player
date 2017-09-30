using CF.MusicLibrary.PandaPlayer.ViewModels.Interfaces;

namespace CF.MusicLibrary.PandaPlayer.ViewModels
{
	public class NavigatedViewModelHolder : INavigatedViewModelHolder
	{
		public IEditDiscPropertiesViewModel EditDiscPropertiesViewModel { get; }

		public IEditSongPropertiesViewModel EditSongPropertiesViewModel { get; }

		public IRateDiscViewModel RateDiscViewModel { get; }

		public IEditDiscArtViewModel EditDiscArtViewModel { get; }

		public ILibraryStatisticsViewModel LibraryStatisticsViewModel { get; }

		public NavigatedViewModelHolder(IEditDiscPropertiesViewModel editDiscPropertiesViewModel, IEditSongPropertiesViewModel editSongPropertiesViewModel,
			IRateDiscViewModel rateDiscViewModel, IEditDiscArtViewModel editDiscArtViewModel, ILibraryStatisticsViewModel libraryStatisticsViewModel)
		{
			EditDiscPropertiesViewModel = editDiscPropertiesViewModel;
			EditSongPropertiesViewModel = editSongPropertiesViewModel;
			RateDiscViewModel = rateDiscViewModel;
			EditDiscArtViewModel = editDiscArtViewModel;
			LibraryStatisticsViewModel = libraryStatisticsViewModel;
		}
	}
}
