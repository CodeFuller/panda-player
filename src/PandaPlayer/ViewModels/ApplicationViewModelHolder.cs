using PandaPlayer.ViewModels.Interfaces;

namespace PandaPlayer.ViewModels
{
	internal class ApplicationViewModelHolder : IApplicationViewModelHolder
	{
		public ILibraryExplorerViewModel LibraryExplorerViewModel { get; }

		public IDiscAdviserViewModel DiscAdviserViewModel { get; }

		public IDiscImageViewModel DiscImageViewModel { get; }

		public ILoggerViewModel LoggerViewModel { get; }

		public ApplicationViewModelHolder(ILibraryExplorerViewModel libraryExplorerViewModel, IDiscAdviserViewModel discAdviserViewModel,
			IDiscImageViewModel discImageViewModel, ILoggerViewModel loggerViewModel)
		{
			LibraryExplorerViewModel = libraryExplorerViewModel;
			DiscAdviserViewModel = discAdviserViewModel;
			DiscImageViewModel = discImageViewModel;
			LoggerViewModel = loggerViewModel;
		}
	}
}
