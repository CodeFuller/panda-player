using CF.MusicLibrary.PandaPlayer.ViewModels.Interfaces;

namespace CF.MusicLibrary.PandaPlayer.ViewModels
{
	public class ApplicationViewModelHolder : IApplicationViewModelHolder
	{
		public ILibraryExplorerViewModel LibraryExplorerViewModel { get; }

		public IDiscAdviserViewModel DiscAdviserViewModel { get; }

		public IDiscArtViewModel DiscArtViewModel { get; }

		public ILoggerViewModel LoggerViewModel { get; }

		public ApplicationViewModelHolder(ILibraryExplorerViewModel libraryExplorerViewModel, IDiscAdviserViewModel discAdviserViewModel,
			IDiscArtViewModel discArtViewModel, ILoggerViewModel loggerViewModel)
		{
			LibraryExplorerViewModel = libraryExplorerViewModel;
			DiscAdviserViewModel = discAdviserViewModel;
			DiscArtViewModel = discArtViewModel;
			LoggerViewModel = loggerViewModel;
		}
	}
}
