namespace MusicLibrary.PandaPlayer.ViewModels.Interfaces
{
	public interface INavigatedViewModelHolder
	{
		IEditDiscPropertiesViewModel EditDiscPropertiesViewModel { get; }

		IEditSongPropertiesViewModel EditSongPropertiesViewModel { get; }

		IRateSongsViewModel RateSongsViewModel { get; }

		IEditDiscImageViewModel EditDiscImageViewModel { get; }

		ILibraryCheckerViewModel LibraryCheckerViewModel { get; }

		ILibraryStatisticsViewModel LibraryStatisticsViewModel { get; }
	}
}
