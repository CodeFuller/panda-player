namespace CF.MusicLibrary.PandaPlayer.ViewModels.Interfaces
{
	public interface IApplicationViewModelHolder
	{
		ILibraryExplorerViewModel LibraryExplorerViewModel { get; }

		IDiscAdviserViewModel DiscAdviserViewModel { get; }

		IDiscArtViewModel DiscArtViewModel { get; }

		ILoggerViewModel LoggerViewModel { get; }
	}
}
