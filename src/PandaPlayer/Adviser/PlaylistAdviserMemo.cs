using System.Diagnostics.Contracts;

namespace PandaPlayer.Adviser
{
	internal class PlaylistAdviserMemo
	{
		public int PlaybacksSinceHighlyRatedSongsPlaylist { get; }

		public int PlaybacksSinceFavoriteAdviseGroup { get; }

		// TODO: Change properties to init only and avoid constructor parameters. Will deserialization work correctly?
		public PlaylistAdviserMemo(int playbacksSinceHighlyRatedSongsPlaylist, int playbacksSinceFavoriteAdviseGroup)
		{
			PlaybacksSinceHighlyRatedSongsPlaylist = playbacksSinceHighlyRatedSongsPlaylist;
			PlaybacksSinceFavoriteAdviseGroup = playbacksSinceFavoriteAdviseGroup;
		}

		[Pure]
		public PlaylistAdviserMemo RegisterPlayback(AdvisedPlaylist advisePlayback)
		{
			var newPlaybacksSinceHighlyRatedSongsPlaylist = advisePlayback.AdvisedPlaylistType == AdvisedPlaylistType.HighlyRatedSongs
					? 0
					: PlaybacksSinceHighlyRatedSongsPlaylist + 1;

			var newPlaybacksSinceFavoriteAdviseGroup = advisePlayback.AdvisedPlaylistType == AdvisedPlaylistType.AdviseSetFromFavoriteAdviseGroup
				? 0
				: PlaybacksSinceFavoriteAdviseGroup + 1;

			return new PlaylistAdviserMemo(newPlaybacksSinceHighlyRatedSongsPlaylist, newPlaybacksSinceFavoriteAdviseGroup);
		}
	}
}
