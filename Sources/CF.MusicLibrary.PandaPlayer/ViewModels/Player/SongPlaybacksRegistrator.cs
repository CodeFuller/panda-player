using System;
using System.Threading.Tasks;
using CF.Library.Core.Facades;
using CF.MusicLibrary.Core.Interfaces;
using CF.MusicLibrary.Core.Objects;
using CF.MusicLibrary.LastFM;
using CF.MusicLibrary.LastFM.Objects;

namespace CF.MusicLibrary.PandaPlayer.ViewModels.Player
{
	public class SongPlaybacksRegistrator : ISongPlaybacksRegistrator
	{
		private readonly IMusicLibrary musicLibrary;
		private readonly IScrobbler scrobbler;
		private readonly IClock clock;

		public SongPlaybacksRegistrator(IMusicLibrary musicLibrary, IScrobbler scrobbler, IClock clock)
		{
			this.musicLibrary = musicLibrary ?? throw new ArgumentNullException(nameof(musicLibrary));
			this.scrobbler = scrobbler ?? throw new ArgumentNullException(nameof(scrobbler));
			this.clock = clock ?? throw new ArgumentNullException(nameof(clock));
		}

		public async Task RegisterPlaybackStart(Song song)
		{
			await scrobbler.UpdateNowPlaying(GetTrackFromSong(song));
		}

		public async Task RegisterPlaybackFinish(Song song)
		{
			var playbackDateTime = clock.Now;
			song.AddPlayback(playbackDateTime);
			await musicLibrary.AddSongPlayback(song, playbackDateTime);

			var scrobble = new TrackScrobble
			{
				Track = GetTrackFromSong(song),
				PlayStartTimestamp = playbackDateTime - song.Duration,
				ChosenByUser = true,
			};

			await scrobbler.Scrobble(scrobble);
		}

		private static Track GetTrackFromSong(Song song)
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
