using System.Threading.Tasks;
using CF.MusicLibrary.BL.Objects;

namespace CF.MusicLibrary.PandaPlayer.ViewModels.Interfaces
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
