using System;
using System.Collections.Generic;
using System.Linq;
using CF.MusicLibrary.BL.Objects;
using CF.MusicLibrary.PandaPlayer.ContentUpdate;
using CF.MusicLibrary.PandaPlayer.Events;
using GalaSoft.MvvmLight.Messaging;
using static CF.Library.Core.Application;
using static CF.Library.Core.Extensions.FormattableStringExtensions;

namespace CF.MusicLibrary.PandaPlayer.ViewModels.PersistentPlaylist
{
	public class PersistentSongPlaylistViewModel : SongPlaylistViewModel
	{
		private readonly IPlaylistDataRepository playlistDataRepository;

		public PersistentSongPlaylistViewModel(ILibraryContentUpdater libraryContentUpdater, IViewNavigator viewNavigator, IPlaylistDataRepository playlistDataRepository)
			: base(libraryContentUpdater, viewNavigator)
		{
			if (playlistDataRepository == null)
			{
				throw new ArgumentNullException(nameof(playlistDataRepository));
			}

			this.playlistDataRepository = playlistDataRepository;

			Messenger.Default.Register<LibraryLoadedEventArgs>(this, e => Load(e.DiscLibrary));
			Messenger.Default.Register<PlaylistFinishedEventArgs>(this, e => this.playlistDataRepository.Purge());
		}

		private void Load(DiscLibrary library)
		{
			PlaylistData playListData = playlistDataRepository.Load();
			if (playListData == null)
			{
				Logger.WriteInfo("No previous playlist data detected");
				return;
			}

			Load(playListData, library);
		}

		private void Load(PlaylistData playListData, DiscLibrary library)
		{
			var songs = new List<Song>();
			foreach (var playlistSong in playListData.Songs)
			{
				var song = library.Songs.SingleOrDefault(s => playlistSong.Matches(s));
				if (song == null)
				{
					Logger.WriteWarning(Current($"Song {playlistSong.Uri} from saved playlist was not found in library. Ignoring saved playlist"));
					return;
				}

				songs.Add(song);
			}

			if (playListData.CurrentSongIndex != null && (playListData.CurrentSongIndex < 0 || playListData.CurrentSongIndex >= songs.Count))
			{
				Logger.WriteWarning(Current($"Index of current song in saved playlist is invalid ({playListData.CurrentSongIndex}, [{0}, {songs.Count})). Ignoring saved playlist"));
				return;
			}

			SetSongsRaw(songs);
			CurrentSongIndex = playListData.CurrentSongIndex;

			Messenger.Default.Send(new PlaylistLoadedEventArgs(this));
		}

		protected override void OnPlaylistChanged()
		{
			playlistDataRepository.Save(new PlaylistData(this));
			base.OnPlaylistChanged();
		}
	}
}
