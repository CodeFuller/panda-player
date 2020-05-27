using System;

namespace MusicLibrary.PandaPlayer.Adviser
{
	internal class PlaylistAdviserMemo : ICloneable
	{
		public int PlaybacksSinceHighlyRatedSongsPlaylist { get; set; }

		public int PlaybacksSinceFavouriteArtistDisc { get; set; }

		public void RegisterPlayback(AdvisedPlaylist advisePlayback)
		{
			if (advisePlayback.AdvisedPlaylistType == AdvisedPlaylistType.FavouriteArtistDisc)
			{
				PlaybacksSinceFavouriteArtistDisc = -1;
			}
			else if (advisePlayback.AdvisedPlaylistType == AdvisedPlaylistType.HighlyRatedSongs)
			{
				PlaybacksSinceHighlyRatedSongsPlaylist = -1;
			}

			++PlaybacksSinceFavouriteArtistDisc;
			++PlaybacksSinceHighlyRatedSongsPlaylist;
		}

		// TODO: Can we get rid of it?
		public object Clone()
		{
			return MemberwiseClone();
		}
	}
}
