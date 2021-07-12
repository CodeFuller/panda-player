using PandaPlayer.DiscAdder.ViewModels.Interfaces;

namespace PandaPlayer.ViewModels.Interfaces
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
