using CF.MusicLibrary.PandaPlayer.ViewModels.Interfaces;

namespace CF.MusicLibrary.PandaPlayer.ViewModels
{
	public class ViewModelHolder : IViewModelHolder
	{
		public ILibraryExplorerViewModel LibraryExplorerViewModel { get; }

		public IDiscAdviserViewModel DiscAdviserViewModel { get; }

		public ILoggerViewModel LoggerViewModel { get; }

		public IEditDiscPropertiesViewModel EditDiscPropertiesViewModel { get; }

		public IEditSongPropertiesViewModel EditSongPropertiesViewModel { get; }

		public IRateDiscViewModel RateDiscViewModel { get; }

		public ViewModelHolder(ILibraryExplorerViewModel libraryExplorerViewModel, IDiscAdviserViewModel discAdviserViewModel,
			IEditDiscPropertiesViewModel editDiscPropertiesViewModel, IEditSongPropertiesViewModel editSongPropertiesViewModel,
			IRateDiscViewModel rateDiscViewModel, LoggerViewModel loggerViewModel)
		{
			LibraryExplorerViewModel = libraryExplorerViewModel;
			DiscAdviserViewModel = discAdviserViewModel;
			LoggerViewModel = loggerViewModel;
			EditDiscPropertiesViewModel = editDiscPropertiesViewModel;
			EditSongPropertiesViewModel = editSongPropertiesViewModel;
			RateDiscViewModel = rateDiscViewModel;
		}
	}
}
