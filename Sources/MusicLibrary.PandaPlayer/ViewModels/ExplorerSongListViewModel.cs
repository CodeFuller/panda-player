using System.Windows.Input;
using CF.Library.Core.Interfaces;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MusicLibrary.PandaPlayer.ContentUpdate;
using MusicLibrary.PandaPlayer.Events.SongEvents;
using MusicLibrary.PandaPlayer.ViewModels.Interfaces;

namespace MusicLibrary.PandaPlayer.ViewModels
{
	public class ExplorerSongListViewModel : SongListViewModel, IExplorerSongListViewModel
	{
		public override bool DisplayTrackNumbers => true;

		public override ICommand PlayFromSongCommand { get; }

		public ExplorerSongListViewModel(ILibraryContentUpdater libraryContentUpdater, IViewNavigator viewNavigator, IWindowService windowService)
			: base(libraryContentUpdater, viewNavigator, windowService)
		{
			PlayFromSongCommand = new RelayCommand(PlayDiscFromSong);
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
