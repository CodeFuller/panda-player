using System.Collections.Generic;

namespace PandaPlayer.Adviser.PlaylistAdvisers
{
	internal class CompositeAdvisedPlaylistQueue
	{
		private readonly Queue<AdvisedPlaylist> highlyRatedSongsAdvises;
		private readonly Queue<AdvisedPlaylist> favoriteArtistDiscsAdvises;
		private readonly Queue<AdvisedPlaylist> rankedAdvises;

		public CompositeAdvisedPlaylistQueue(
			IEnumerable<AdvisedPlaylist> highlyRatedSongsAdvises,
			IEnumerable<AdvisedPlaylist> favoriteArtistDiscsAdvises, IEnumerable<AdvisedPlaylist> rankedAdvises)
		{
			this.highlyRatedSongsAdvises = new Queue<AdvisedPlaylist>(highlyRatedSongsAdvises);
			this.favoriteArtistDiscsAdvises = new Queue<AdvisedPlaylist>(favoriteArtistDiscsAdvises);
			this.rankedAdvises = new Queue<AdvisedPlaylist>(rankedAdvises);
		}

		public bool TryDequeueHighlyRatedSongsAdvise(out AdvisedPlaylist currentAdvise)
		{
			return highlyRatedSongsAdvises.TryDequeue(out currentAdvise);
		}

		public bool TryDequeueFavoriteArtistDiscsAdvise(out AdvisedPlaylist currentAdvise)
		{
			return favoriteArtistDiscsAdvises.TryDequeue(out currentAdvise);
		}

		public bool TryDequeueRankedAdvise(out AdvisedPlaylist currentAdvise)
		{
			return rankedAdvises.TryDequeue(out currentAdvise);
		}
	}
}
