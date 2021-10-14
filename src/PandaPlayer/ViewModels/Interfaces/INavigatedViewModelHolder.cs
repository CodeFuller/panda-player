using PandaPlayer.DiscAdder.ViewModels.Interfaces;

namespace PandaPlayer.ViewModels.Interfaces
{
	public interface INavigatedViewModelHolder
	{
		ICreateAdviseGroupViewModel CreateAdviseGroupViewModel { get; }

		IEditDiscPropertiesViewModel EditDiscPropertiesViewModel { get; }

		IEditSongPropertiesViewModel EditSongPropertiesViewModel { get; }

		IRateSongsViewModel RateSongsViewModel { get; }

		IEditDiscImageViewModel EditDiscImageViewModel { get; }

		IAdviseSetsEditorViewModel AdviseSetsEditorViewModel { get; }

		IDiscAdderViewModel DiscAdderViewModel { get; }

		ILibraryCheckerViewModel LibraryCheckerViewModel { get; }

		ILibraryStatisticsViewModel LibraryStatisticsViewModel { get; }

		IDeleteContentViewModel DeleteContentViewModel { get; }
	}
}
