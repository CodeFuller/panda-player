using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Extensions.Logging;
using MusicLibrary.Core.Models;
using MusicLibrary.PandaPlayer.Events;
using MusicLibrary.PandaPlayer.Events.SongListEvents;
using MusicLibrary.Services.Interfaces;

namespace MusicLibrary.PandaPlayer.ViewModels.PersistentPlaylist
{
	public class PersistentSongPlaylistViewModel : SongPlaylistViewModel
	{
		private const string SongPlaylistDataKey = "SongPlaylistData";

		private readonly ISongsService songsService;
		private readonly ISessionDataService sessionDataService;
		private readonly ILogger<PersistentSongPlaylistViewModel> logger;

		public PersistentSongPlaylistViewModel(ISongsService songsService, IViewNavigator viewNavigator,
			ISessionDataService sessionDataService, ILogger<PersistentSongPlaylistViewModel> logger)
			: base(songsService, viewNavigator)
		{
			this.songsService = songsService ?? throw new ArgumentNullException(nameof(songsService));
			this.sessionDataService = sessionDataService ?? throw new ArgumentNullException(nameof(sessionDataService));
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

			Messenger.Default.Register<ApplicationLoadedEventArgs>(this, _ => Load(CancellationToken.None));
			Messenger.Default.Register<PlaylistFinishedEventArgs>(this, _ => OnPlaylistFinished(CancellationToken.None));
		}

		private async void Load(CancellationToken cancellationToken)
		{
			var (songs, songIndex) = await LoadPlaylistData(cancellationToken);
			if (songs != null)
			{
				SetSongs(songs);
				SetCurrentSong(songIndex);

				Messenger.Default.Send(new PlaylistLoadedEventArgs(Songs, CurrentSong, CurrentSongIndex));
			}
			else
			{
				Messenger.Default.Send(new NoPlaylistLoadedEventArgs());
			}
		}

		private async Task<(IReadOnlyCollection<SongModel> Songs, int? CurrentSongIndex)> LoadPlaylistData(CancellationToken cancellationToken)
		{
			var playListData = await sessionDataService.GetData<PlaylistData>(SongPlaylistDataKey, cancellationToken);
			if (playListData == null)
			{
				logger.LogInformation("No previous playlist data loaded");
				return (null, null);
			}

			return await LoadPlaylistSongs(playListData, cancellationToken);
		}

		private async Task<(IReadOnlyCollection<SongModel> Songs, int? CurrentSongIndex)> LoadPlaylistSongs(PlaylistData playListData, CancellationToken cancellationToken)
		{
			var songIds = playListData.Songs
				.Select(s => s.Id)
				.Select(id => new ItemId(id))
				.Distinct();

			var loadedSongs = (await songsService.GetSongs(songIds, cancellationToken))
				.ToDictionary(s => s.Id, s => s);

			var newSongIndex = playListData.CurrentSongIndex;

			var playListSongs = new List<SongModel>();
			foreach (var (playlistSong, songIndex) in playListData.Songs.Select((song, i) => (song, i)))
			{
				if (!loadedSongs.TryGetValue(new ItemId(playlistSong.Id), out var loadedSong))
				{
					logger.LogWarning($"Song {playlistSong.Id} from saved playlist was not found in library. Ignoring saved playlist.");
					return (null, null);
				}

				if (loadedSong.IsDeleted)
				{
					logger.LogWarning($"Song '{GetSongTitle(loadedSong)}' from saved playlist was deleted from the library. Ignoring this song.");

					if (songIndex < newSongIndex)
					{
						--newSongIndex;
					}

					continue;
				}

				playListSongs.Add(loadedSong);
			}

			if (newSongIndex != null && (newSongIndex < 0 || newSongIndex >= playListSongs.Count))
			{
				logger.LogWarning($"Index of current song in saved playlist is invalid ({newSongIndex}, [0, {playListSongs.Count})). Ignoring saved playlist.");
				return (null, null);
			}

			return (playListSongs, newSongIndex);
		}

		private static string GetSongTitle(SongModel song)
		{
			var sb = new StringBuilder();
			if (song.TrackNumber.HasValue)
			{
				sb.Append($"{song.TrackNumber.Value:D2} - ");
			}

			sb.Append(song.Title);

			return sb.ToString();
		}

		private async void OnPlaylistFinished(CancellationToken cancellationToken)
		{
			await sessionDataService.PurgeData(SongPlaylistDataKey, cancellationToken);
		}

		protected override async Task OnPlaylistChanged(CancellationToken cancellationToken)
		{
			var playlistData = new PlaylistData(Songs, CurrentSongIndex);

			await sessionDataService.SaveData(SongPlaylistDataKey, playlistData, cancellationToken);

			await base.OnPlaylistChanged(cancellationToken);
		}
	}
}
