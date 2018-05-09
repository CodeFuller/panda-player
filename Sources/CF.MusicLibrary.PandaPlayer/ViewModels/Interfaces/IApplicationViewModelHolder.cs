namespace CF.MusicLibrary.PandaPlayer.ViewModels.Interfaces
{
	public interface IApplicationViewModelHolder
	{
		ILibraryExplorerViewModel LibraryExplorerViewModel { get; }

		IDiscAdviserViewModel DiscAdviserViewModel { get; }

		IDiscImageViewModel DiscImageViewModel { get; }

		ILoggerViewModel LoggerViewModel { get; }
	}
}
