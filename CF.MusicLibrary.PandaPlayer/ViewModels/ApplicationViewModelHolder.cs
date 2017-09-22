using CF.MusicLibrary.PandaPlayer.ViewModels.Interfaces;

namespace CF.MusicLibrary.PandaPlayer.ViewModels
{
	public class ApplicationViewModelHolder : IApplicationViewModelHolder
	{
		public ILibraryExplorerViewModel LibraryExplorerViewModel { get; }

		public IDiscAdviserViewModel DiscAdviserViewModel { get; }

		public ILoggerViewModel LoggerViewModel { get; }

		public ApplicationViewModelHolder(ILibraryExplorerViewModel libraryExplorerViewModel, IDiscAdviserViewModel discAdviserViewModel, ILoggerViewModel loggerViewModel)
		{
			LibraryExplorerViewModel = libraryExplorerViewModel;
			DiscAdviserViewModel = discAdviserViewModel;
			LoggerViewModel = loggerViewModel;
		}
	}
}
