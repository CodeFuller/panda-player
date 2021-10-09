using System.Collections.Generic;

namespace PandaPlayer.Adviser.PlaylistAdvisers
{
	internal class CompositeAdvisedPlaylistQueue
	{
		private readonly Queue<AdvisedPlaylist> highlyRatedSongsAdvises;
		private readonly Queue<AdvisedPlaylist> favoriteAdviseGroupsAdvises;
		private readonly Queue<AdvisedPlaylist> rankedAdvises;

		public CompositeAdvisedPlaylistQueue(
			IEnumerable<AdvisedPlaylist> highlyRatedSongsAdvises,
			IEnumerable<AdvisedPlaylist> favoriteAdviseGroupsAdvises, IEnumerable<AdvisedPlaylist> rankedAdvises)
		{
			this.highlyRatedSongsAdvises = new Queue<AdvisedPlaylist>(highlyRatedSongsAdvises);
			this.favoriteAdviseGroupsAdvises = new Queue<AdvisedPlaylist>(favoriteAdviseGroupsAdvises);
			this.rankedAdvises = new Queue<AdvisedPlaylist>(rankedAdvises);
		}

		public bool TryDequeueHighlyRatedSongsAdvise(out AdvisedPlaylist currentAdvise)
		{
			return highlyRatedSongsAdvises.TryDequeue(out currentAdvise);
		}

		public bool TryDequeueFavoriteAdviseGroupAdvise(out AdvisedPlaylist currentAdvise)
		{
			return favoriteAdviseGroupsAdvises.TryDequeue(out currentAdvise);
		}

		public bool TryDequeueRankedAdvise(out AdvisedPlaylist currentAdvise)
		{
			return rankedAdvises.TryDequeue(out currentAdvise);
		}
	}
}
