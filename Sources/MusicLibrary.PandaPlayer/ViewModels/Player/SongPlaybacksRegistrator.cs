using System;
using System.Threading;
using System.Threading.Tasks;
using CF.Library.Core.Facades;
using MusicLibrary.Core.Interfaces.Services;
using MusicLibrary.Core.Models;
using MusicLibrary.LastFM;
using MusicLibrary.LastFM.Objects;

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

		public async Task RegisterPlaybackStart(SongModel song, CancellationToken cancellationToken)
		{
			await scrobbler.UpdateNowPlaying(GetTrackFromSong(song));
		}

		public async Task RegisterPlaybackFinish(SongModel song, CancellationToken cancellationToken)
		{
			// TBD: IClock returns DateTime, not DateTimeOffset
			var playbackDateTime = clock.Now;
			await songsService.AddSongPlayback(song, playbackDateTime, cancellationToken);

			var scrobble = new TrackScrobble
			{
				Track = GetTrackFromSong(song),
				PlayStartTimestamp = playbackDateTime - song.Duration,
				ChosenByUser = true,
			};

			await scrobbler.Scrobble(scrobble);
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
