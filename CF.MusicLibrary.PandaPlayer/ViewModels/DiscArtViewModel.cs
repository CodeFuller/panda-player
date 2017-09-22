using System;
using CF.MusicLibrary.BL.Interfaces;
using CF.MusicLibrary.BL.Objects;
using CF.MusicLibrary.PandaPlayer.Events;
using CF.MusicLibrary.PandaPlayer.ViewModels.Interfaces;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;

namespace CF.MusicLibrary.PandaPlayer.ViewModels
{
	public class DiscArtViewModel : ViewModelBase, IDiscArtViewModel
	{
		private readonly IMusicLibrary musicLibrary;

		private Song currentSong;
		private Song CurrentSong
		{
			get { return currentSong; }
			set
			{
				Set(ref currentSong, value);
				CurrImageFileName = GetCurrImageFileName();
			}
		}

		private Disc currentDisc;
		private Disc CurrentDisc
		{
			get { return currentDisc; }
			set
			{
				Set(ref currentDisc, value);
				CurrImageFileName = GetCurrImageFileName();
			}
		}

		private string currImageFileName;
		public string CurrImageFileName
		{
			get { return currImageFileName; }
			set { Set(ref currImageFileName, value); }
		}

		public DiscArtViewModel(IMusicLibrary musicLibrary)
		{
			if (musicLibrary == null)
			{
				throw new ArgumentNullException(nameof(musicLibrary));
			}

			this.musicLibrary = musicLibrary;

			Messenger.Default.Register<NewSongPlaybackStartedEventArgs>(this, e => CurrentSong = e.Song);
			Messenger.Default.Register<PlaylistFinishedEventArgs>(this, e => CurrentSong = null);
			Messenger.Default.Register<LibraryExplorerDiscChangedEventArgs>(this, e => CurrentDisc = e.Disc);
			Messenger.Default.Register<LibraryExplorerFolderChangedEventArgs>(this, e => CurrentDisc = null);
		}

		private string GetCurrImageFileName()
		{
			var activeDisc = CurrentSong?.Disc ?? CurrentDisc;
			return activeDisc == null ? null : musicLibrary.GetDiscCoverImage(activeDisc).Result;
		}
	}
}
