using System.Threading.Tasks;
using MusicLibrary.Core.Objects;

namespace MusicLibrary.PandaPlayer.ViewModels.Interfaces
{
	public interface IEditDiscPropertiesViewModel
	{
		string FolderName { get; set; }

		string DiscTitle { get; set; }

		string AlbumTitle { get; set; }

		void Load(Disc disc);

		Task Save();
	}
}
