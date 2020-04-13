using System.Threading.Tasks;
using MusicLibrary.Core.Objects;

namespace MusicLibrary.PandaPlayer.ViewModels.Interfaces
{
	public interface IRateDiscViewModel
	{
		void Load(Disc disc);

		Task Save();
	}
}
