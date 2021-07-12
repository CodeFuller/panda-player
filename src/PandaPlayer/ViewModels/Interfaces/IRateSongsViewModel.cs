using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PandaPlayer.Core.Models;

namespace PandaPlayer.ViewModels.Interfaces
{
	public interface IRateSongsViewModel
	{
		void Load(IEnumerable<SongModel> songs);

		Task Save(CancellationToken cancellationToken);
	}
}
