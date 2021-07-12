using System.Collections.Generic;

namespace PandaPlayer.Adviser.PlaylistAdvisers
{
	internal class CompositeAdvisedPlaylistQueue
	{
		private readonly Queue<AdvisedPlaylist> highlyRatedSongsAdvises;
		private readonly Queue<AdvisedPlaylist> favoriteArtistDiscsAdvises;
		private readonly Queue<AdvisedPlaylist> rankedDiscsAdvises;

		public CompositeAdvisedPlaylistQueue(
			IEnumerable<AdvisedPlaylist> highlyRatedSongsAdvises,
			IEnumerable<AdvisedPlaylist> favoriteArtistDiscsAdvises, IEnumerable<AdvisedPlaylist> rankedDiscsAdvises)
		{
			this.highlyRatedSongsAdvises = new Queue<AdvisedPlaylist>(highlyRatedSongsAdvises);
			this.favoriteArtistDiscsAdvises = new Queue<AdvisedPlaylist>(favoriteArtistDiscsAdvises);
			this.rankedDiscsAdvises = new Queue<AdvisedPlaylist>(rankedDiscsAdvises);
		}

		public bool TryDequeueHighlyRatedSongsAdvise(out AdvisedPlaylist currentAdvise)
		{
			return highlyRatedSongsAdvises.TryDequeue(out currentAdvise);
		}

		public bool TryDequeueFavoriteArtistDiscsAdvise(out AdvisedPlaylist currentAdvise)
		{
			return favoriteArtistDiscsAdvises.TryDequeue(out currentAdvise);
		}

		public bool TryDequeueRankedDiscsAdvise(out AdvisedPlaylist currentAdvise)
		{
			return rankedDiscsAdvises.TryDequeue(out currentAdvise);
		}
	}
}
