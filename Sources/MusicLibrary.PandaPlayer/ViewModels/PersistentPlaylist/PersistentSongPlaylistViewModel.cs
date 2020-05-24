using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CF.Library.Core.Interfaces;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Extensions.Logging;
using MusicLibrary.Core.Models;
using MusicLibrary.PandaPlayer.Events;
using MusicLibrary.PandaPlayer.Events.SongListEvents;
using MusicLibrary.Services.Interfaces;
using static CF.Library.Core.Extensions.FormattableStringExtensions;

namespace MusicLibrary.PandaPlayer.ViewModels.PersistentPlaylist
{
	public class PersistentSongPlaylistViewModel : SongPlaylistViewModel
	{
		private readonly ISongsService songsService;

		private readonly IGenericDataRepository<PlaylistData> playlistDataRepository;
		private readonly ILogger<PersistentSongPlaylistViewModel> logger;

		public PersistentSongPlaylistViewModel(ISongsService songsService, IViewNavigator viewNavigator, IWindowService windowService,
			IGenericDataRepository<PlaylistData> playlistDataRepository, ILogger<PersistentSongPlaylistViewModel> logger)
			: base(songsService, viewNavigator, windowService)
		{
			this.songsService = songsService ?? throw new ArgumentNullException(nameof(songsService));
			this.playlistDataRepository = playlistDataRepository ?? throw new ArgumentNullException(nameof(playlistDataRepository));
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

			Messenger.Default.Register<ApplicationLoadedEventArgs>(this, e => Load());
			Messenger.Default.Register<PlaylistFinishedEventArgs>(this, e => this.playlistDataRepository.Purge());
		}

		private void Load()
		{
			PlaylistData playListData = playlistDataRepository.Load();
			if (playListData == null)
			{
				logger.LogInformation("No previous playlist data detected");
				return;
			}

			Load(playListData);
		}

		private void Load(PlaylistData playListData)
		{
			var songIds = playListData.Songs
				.Select(s => s.Id)
				.Select(id => new ItemId(id))
				.Distinct();

			// TBD: Make async
			var loadedSongs = songsService.GetSongs(songIds, CancellationToken.None).Result
				.ToDictionary(s => s.Id, s => s);

			var playListSongs = new List<SongModel>();
			foreach (var playlistSong in playListData.Songs)
			{
				if (!loadedSongs.TryGetValue(new ItemId(playlistSong.Id), out var loadedSong))
				{
					logger.LogInformation(Current($"Song {playlistSong.Id} from saved playlist was not found in library. Ignoring saved playlist"));
					return;
				}

				playListSongs.Add(loadedSong);
			}

			if (playListData.CurrentSongIndex != null && (playListData.CurrentSongIndex < 0 || playListData.CurrentSongIndex >= loadedSongs.Count))
			{
				logger.LogInformation(Current($"Index of current song in saved playlist is invalid ({playListData.CurrentSongIndex}, [{0}, {loadedSongs.Count})). Ignoring saved playlist"));
				return;
			}

			SetSongsRaw(playListSongs);
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
