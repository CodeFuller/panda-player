using System.Collections.Generic;
using System.Threading.Tasks;
using MusicLibrary.Core.Objects;

namespace MusicLibrary.PandaPlayer.ViewModels.Interfaces
{
	public interface IEditSongPropertiesViewModel
	{
		void Load(IEnumerable<Song> songs);

		Task Save();
	}
}
