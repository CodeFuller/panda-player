using System.Threading.Tasks;
using CF.MusicLibrary.BL.Objects;

namespace CF.MusicLibrary.PandaPlayer.ViewModels.Interfaces
{
	public interface IRateDiscViewModel
	{
		void Load(Disc disc);

		Task Save();
	}
}
