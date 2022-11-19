using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PandaPlayer.Core.Models;

namespace PandaPlayer.ViewModels.Interfaces
{
	public interface IRateSongsViewModel
	{
		IEnumerable<RatingModel> AvailableRatings { get; }

		RatingModel SelectedRating { get; set; }

		void Load(IEnumerable<SongModel> songs);

		Task Save(CancellationToken cancellationToken);
	}
}
