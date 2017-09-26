using System;
using System.Threading.Tasks;
using CF.MusicLibrary.BL.Interfaces;
using CF.MusicLibrary.BL.Objects;
using CF.MusicLibrary.PandaPlayer.Events;
using CF.MusicLibrary.PandaPlayer.ViewModels.Interfaces;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;

namespace CF.MusicLibrary.PandaPlayer.ViewModels.DiscArt
{
	public class DiscArtViewModel : ViewModelBase, IDiscArtViewModel
	{
		private readonly IMusicLibrary musicLibrary;
		private readonly IViewNavigator viewNavigator;

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

		private Disc currentExplorerDisc;
		private Disc CurrentExplorerDisc
		{
			get { return currentExplorerDisc; }
			set
			{
				Set(ref currentExplorerDisc, value);
				CurrImageFileName = GetCurrImageFileName();
			}
		}

		private Disc DisplayedArtDisc => CurrentSong?.Disc ?? CurrentExplorerDisc;

		private string currImageFileName;
		public string CurrImageFileName
		{
			get { return currImageFileName; }
			private set
			{
				//	Why don't we use ViewModelBase.Set(ref currImageFileName, value) ?
				//	When disc art is updated with new image, CurrImageFileName is not actually changed, however
				//	we need PropertyChanged event to be fired so that Image control updated image in the view.
				//	Seems like ViewModelBase.Set() has some internal check whether new value equals to the old one
				//	and don't fire the event in this case. That's why we should raise event manually.
				currImageFileName = value;
				RaisePropertyChanged();
			}
		}

		public DiscArtViewModel(IMusicLibrary musicLibrary, IViewNavigator viewNavigator)
		{
			if (musicLibrary == null)
			{
				throw new ArgumentNullException(nameof(musicLibrary));
			}
			if (viewNavigator == null)
			{
				throw new ArgumentNullException(nameof(viewNavigator));
			}

			this.musicLibrary = musicLibrary;
			this.viewNavigator = viewNavigator;

			Messenger.Default.Register<PlaylistChangedEventArgs>(this, e => CurrentSong = e.Playlist.CurrentSong);
			Messenger.Default.Register<PlaylistFinishedEventArgs>(this, e => CurrentSong = null);
			Messenger.Default.Register<LibraryExplorerDiscChangedEventArgs>(this, e => CurrentExplorerDisc = e.Disc);
			Messenger.Default.Register<LibraryExplorerFolderChangedEventArgs>(this, e => CurrentExplorerDisc = null);
			Messenger.Default.Register<DiscArtChangedEventArgs>(this, e => OnDiscArtChanged(e.Disc));
		}

		private string GetCurrImageFileName()
		{
			var activeDisc = DisplayedArtDisc;
			return activeDisc == null ? null : musicLibrary.GetDiscCoverImage(activeDisc).Result;
		}

		public async Task EditDiscArt()
		{
			var activeDisc = DisplayedArtDisc;
			if (activeDisc == null)
			{
				return;
			}

			await viewNavigator.ShowEditDiscArtView(activeDisc);
		}

		private void OnDiscArtChanged(Disc disc)
		{
			if (disc == DisplayedArtDisc)
			{
				CurrImageFileName = GetCurrImageFileName();
			}
		}
	}
}
