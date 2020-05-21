using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MusicLibrary.Logic.Models;

namespace MusicLibrary.PandaPlayer.ViewModels.Interfaces
{
	public interface IEditSongPropertiesViewModel
	{
		Task Load(IEnumerable<SongModel> songs, CancellationToken cancellationToken);

		Task Save(CancellationToken cancellationToken);
	}
}
