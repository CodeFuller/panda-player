using System.Diagnostics.Contracts;

namespace MusicLibrary.PandaPlayer.Adviser
{
	internal class PlaylistAdviserMemo
	{
		public int PlaybacksSinceHighlyRatedSongsPlaylist { get; }

		public int PlaybacksSinceFavoriteArtistDisc { get; }

		public PlaylistAdviserMemo(int playbacksSinceHighlyRatedSongsPlaylist, int playbacksSinceFavoriteArtistDisc)
		{
			PlaybacksSinceHighlyRatedSongsPlaylist = playbacksSinceHighlyRatedSongsPlaylist;
			PlaybacksSinceFavoriteArtistDisc = playbacksSinceFavoriteArtistDisc;
		}

		[Pure]
		public PlaylistAdviserMemo RegisterPlayback(AdvisedPlaylist advisePlayback)
		{
			var newPlaybacksSinceHighlyRatedSongsPlaylist = advisePlayback.AdvisedPlaylistType == AdvisedPlaylistType.HighlyRatedSongs
					? 0
					: PlaybacksSinceHighlyRatedSongsPlaylist + 1;

			var newPlaybacksSinceFavoriteArtistDisc = advisePlayback.AdvisedPlaylistType == AdvisedPlaylistType.FavoriteArtistDisc
				? 0
				: PlaybacksSinceFavoriteArtistDisc + 1;

			return new PlaylistAdviserMemo(newPlaybacksSinceHighlyRatedSongsPlaylist, newPlaybacksSinceFavoriteArtistDisc);
		}
	}
}
