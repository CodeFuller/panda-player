using System.Collections.Generic;
using System.Threading.Tasks;
using CF.MusicLibrary.Core.Objects;

namespace CF.MusicLibrary.PandaPlayer.ViewModels.Interfaces
{
	public interface IEditSongPropertiesViewModel
	{
		void Load(IEnumerable<Song> songs);

		Task Save();
	}
}
