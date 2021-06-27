﻿using System;
using System.Threading;
using System.Threading.Tasks;
using MusicLibrary.Core.Facades;
using MusicLibrary.Core.Models;
using MusicLibrary.LastFM.Interfaces;
using MusicLibrary.LastFM.Objects;
using MusicLibrary.Services.Interfaces;

namespace MusicLibrary.PandaPlayer.ViewModels.Player
{
	public class SongPlaybacksRegistrator : ISongPlaybacksRegistrator
	{
		private readonly ISongsService songsService;
		private readonly IScrobbler scrobbler;
		private readonly IClock clock;

		public SongPlaybacksRegistrator(ISongsService songsService, IScrobbler scrobbler, IClock clock)
		{
			this.songsService = songsService ?? throw new ArgumentNullException(nameof(songsService));
			this.scrobbler = scrobbler ?? throw new ArgumentNullException(nameof(scrobbler));
			this.clock = clock ?? throw new ArgumentNullException(nameof(clock));
		}

		public Task RegisterPlaybackStart(SongModel song, CancellationToken cancellationToken)
		{
			return scrobbler.UpdateNowPlaying(GetTrackFromSong(song), cancellationToken);
		}

		public async Task RegisterPlaybackFinish(SongModel song, CancellationToken cancellationToken)
		{
			var playbackDateTime = clock.Now;
			await songsService.AddSongPlayback(song, playbackDateTime, cancellationToken);

			var scrobble = new TrackScrobble
			{
				Track = GetTrackFromSong(song),
				PlayStartTimestamp = playbackDateTime - song.Duration,
				ChosenByUser = true,
			};

			await scrobbler.Scrobble(scrobble, cancellationToken);
		}

		private static Track GetTrackFromSong(SongModel song)
		{
			return new Track
			{
				Number = song.TrackNumber,
				Title = song.Title,
				Artist = song.Artist?.Name,
				Album = new Album(song.Artist?.Name, song.Disc.AlbumTitle),
				Duration = song.Duration,
			};
		}
	}
}