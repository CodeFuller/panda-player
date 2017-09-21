namespace CF.MusicLibrary.PandaPlayer.ViewModels.Interfaces
{
	public interface IViewModelHolder
	{
		ILibraryExplorerViewModel LibraryExplorerViewModel { get; }

		IDiscAdviserViewModel DiscAdviserViewModel { get; }

		ILoggerViewModel LoggerViewModel { get; }

		IEditDiscPropertiesViewModel EditDiscPropertiesViewModel { get; }

		IEditSongPropertiesViewModel EditSongPropertiesViewModel { get; }

		IRateDiscViewModel RateDiscViewModel { get; }
	}
}
