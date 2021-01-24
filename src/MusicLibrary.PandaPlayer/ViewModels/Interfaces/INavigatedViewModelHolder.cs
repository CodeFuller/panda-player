using MusicLibrary.DiscAdder.ViewModels.Interfaces;

namespace MusicLibrary.PandaPlayer.ViewModels.Interfaces
{
	public interface INavigatedViewModelHolder
	{
		IEditDiscPropertiesViewModel EditDiscPropertiesViewModel { get; }

		IEditSongPropertiesViewModel EditSongPropertiesViewModel { get; }

		IRateSongsViewModel RateSongsViewModel { get; }

		IEditDiscImageViewModel EditDiscImageViewModel { get; }

		IDiscAdderViewModel DiscAdderViewModel { get; }

		ILibraryCheckerViewModel LibraryCheckerViewModel { get; }

		ILibraryStatisticsViewModel LibraryStatisticsViewModel { get; }
	}
}
