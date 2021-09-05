using System;
using PandaPlayer.DiscAdder.ViewModels.Interfaces;
using PandaPlayer.ViewModels.Interfaces;

namespace PandaPlayer.ViewModels
{
	public class NavigatedViewModelHolder : INavigatedViewModelHolder
	{
		public ICreateAdviseGroupViewModel CreateAdviseGroupViewModel { get; }

		public IEditDiscPropertiesViewModel EditDiscPropertiesViewModel { get; }

		public IEditSongPropertiesViewModel EditSongPropertiesViewModel { get; }

		public IRateSongsViewModel RateSongsViewModel { get; }

		public IEditDiscImageViewModel EditDiscImageViewModel { get; }

		public IAdviseSetsEditorViewModel AdviseSetsEditorViewModel { get; }

		public IDiscAdderViewModel DiscAdderViewModel { get; }

		public ILibraryCheckerViewModel LibraryCheckerViewModel { get; }

		public ILibraryStatisticsViewModel LibraryStatisticsViewModel { get; }

		public NavigatedViewModelHolder(ICreateAdviseGroupViewModel createAdviseGroupViewModel, IEditDiscPropertiesViewModel editDiscPropertiesViewModel,
			IEditSongPropertiesViewModel editSongPropertiesViewModel, IRateSongsViewModel rateSongsViewModel, IEditDiscImageViewModel editDiscImageViewModel,
			IAdviseSetsEditorViewModel adviseSetsEditorViewModel, IDiscAdderViewModel discAdderViewModel, ILibraryCheckerViewModel libraryCheckerViewModel, ILibraryStatisticsViewModel libraryStatisticsViewModel)
		{
			CreateAdviseGroupViewModel = createAdviseGroupViewModel ?? throw new ArgumentNullException(nameof(createAdviseGroupViewModel));
			EditDiscPropertiesViewModel = editDiscPropertiesViewModel ?? throw new ArgumentNullException(nameof(editDiscImageViewModel));
			EditSongPropertiesViewModel = editSongPropertiesViewModel ?? throw new ArgumentNullException(nameof(editSongPropertiesViewModel));
			RateSongsViewModel = rateSongsViewModel ?? throw new ArgumentNullException(nameof(rateSongsViewModel));
			EditDiscImageViewModel = editDiscImageViewModel ?? throw new ArgumentNullException(nameof(editDiscImageViewModel));
			AdviseSetsEditorViewModel = adviseSetsEditorViewModel ?? throw new ArgumentNullException(nameof(adviseSetsEditorViewModel));
			DiscAdderViewModel = discAdderViewModel ?? throw new ArgumentNullException(nameof(discAdderViewModel));
			LibraryCheckerViewModel = libraryCheckerViewModel ?? throw new ArgumentNullException(nameof(libraryCheckerViewModel));
			LibraryStatisticsViewModel = libraryStatisticsViewModel ?? throw new ArgumentNullException(nameof(libraryStatisticsViewModel));
		}
	}
}
