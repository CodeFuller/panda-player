namespace PandaPlayer.ViewModels.Interfaces
{
	internal interface IApplicationViewModelHolder
	{
		ILibraryExplorerViewModel LibraryExplorerViewModel { get; }

		IDiscAdviserViewModel DiscAdviserViewModel { get; }

		IDiscImageViewModel DiscImageViewModel { get; }

		ILoggerViewModel LoggerViewModel { get; }
	}
}
