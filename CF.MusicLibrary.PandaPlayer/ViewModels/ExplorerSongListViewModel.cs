using System.Windows.Input;
using CF.MusicLibrary.PandaPlayer.ContentUpdate;
using CF.MusicLibrary.PandaPlayer.Events;
using CF.MusicLibrary.PandaPlayer.ViewModels.Interfaces;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace CF.MusicLibrary.PandaPlayer.ViewModels
{
	public class ExplorerSongListViewModel : SongListViewModel, IExplorerSongListViewModel
	{
		public override bool DisplayTrackNumbers => true;

		public ICommand PlayDiscFromSongCommand { get; }

		public ExplorerSongListViewModel(ILibraryContentUpdater libraryContentUpdater)
			: base(libraryContentUpdater)
		{
			PlayDiscFromSongCommand = new RelayCommand(PlayDiscFromSong);
		}

		private void PlayDiscFromSong()
		{
			var selectedSong = SelectedSongItem?.Song;
			if (selectedSong != null)
			{
				Messenger.Default.Send(new PlayDiscFromSongEventArgs(selectedSong));
			}
		}
	}
}
