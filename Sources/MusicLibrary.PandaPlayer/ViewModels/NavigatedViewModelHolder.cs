using System;
using MusicLibrary.DiscAdder.ViewModels.Interfaces;
using MusicLibrary.PandaPlayer.ViewModels.Interfaces;

namespace MusicLibrary.PandaPlayer.ViewModels
{
	public class NavigatedViewModelHolder : INavigatedViewModelHolder
	{
		public IEditDiscPropertiesViewModel EditDiscPropertiesViewModel { get; }

		public IEditSongPropertiesViewModel EditSongPropertiesViewModel { get; }

		public IRateSongsViewModel RateSongsViewModel { get; }

		public IEditDiscImageViewModel EditDiscImageViewModel { get; }

		public IDiscAdderViewModel DiscAdderViewModel { get; }

		public ILibraryCheckerViewModel LibraryCheckerViewModel { get; }

		public ILibraryStatisticsViewModel LibraryStatisticsViewModel { get; }

		public NavigatedViewModelHolder(IEditDiscPropertiesViewModel editDiscPropertiesViewModel, IEditSongPropertiesViewModel editSongPropertiesViewModel,
			IRateSongsViewModel rateSongsViewModel, IEditDiscImageViewModel editDiscImageViewModel, IDiscAdderViewModel discAdderViewModel,
			ILibraryCheckerViewModel libraryCheckerViewModel, ILibraryStatisticsViewModel libraryStatisticsViewModel)
		{
			EditDiscPropertiesViewModel = editDiscPropertiesViewModel ?? throw new ArgumentNullException(nameof(editDiscImageViewModel));
			EditSongPropertiesViewModel = editSongPropertiesViewModel ?? throw new ArgumentNullException(nameof(editSongPropertiesViewModel));
			RateSongsViewModel = rateSongsViewModel ?? throw new ArgumentNullException(nameof(rateSongsViewModel));
			EditDiscImageViewModel = editDiscImageViewModel ?? throw new ArgumentNullException(nameof(editDiscImageViewModel));
			DiscAdderViewModel = discAdderViewModel ?? throw new ArgumentNullException(nameof(discAdderViewModel));
			LibraryCheckerViewModel = libraryCheckerViewModel ?? throw new ArgumentNullException(nameof(libraryCheckerViewModel));
			LibraryStatisticsViewModel = libraryStatisticsViewModel ?? throw new ArgumentNullException(nameof(libraryStatisticsViewModel));
		}
	}
}
