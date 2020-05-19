using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MusicLibrary.Core.Objects;

namespace MusicLibrary.PandaPlayer.ViewModels.Interfaces
{
	public interface IEditSongPropertiesViewModel
	{
		Task Load(IEnumerable<Song> songs, CancellationToken cancellationToken);

		Task Save();
	}
}
