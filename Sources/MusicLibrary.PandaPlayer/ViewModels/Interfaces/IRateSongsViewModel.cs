using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MusicLibrary.Logic.Models;

namespace MusicLibrary.PandaPlayer.ViewModels.Interfaces
{
	public interface IRateSongsViewModel
	{
		void Load(IEnumerable<SongModel> songs);

		Task Save(CancellationToken cancellationToken);
	}
}
