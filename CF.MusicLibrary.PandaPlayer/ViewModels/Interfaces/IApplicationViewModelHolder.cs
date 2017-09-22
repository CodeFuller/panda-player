namespace CF.MusicLibrary.PandaPlayer.ViewModels.Interfaces
{
	public interface IApplicationViewModelHolder
	{
		ILibraryExplorerViewModel LibraryExplorerViewModel { get; }

		IDiscAdviserViewModel DiscAdviserViewModel { get; }

		ILoggerViewModel LoggerViewModel { get; }
	}
}
