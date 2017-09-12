using System.Threading.Tasks;
using CF.MusicLibrary.BL.Objects;

namespace CF.MusicLibrary.PandaPlayer
{
	public interface ILibraryContentUpdater
	{
		Task DeleteDisc(Disc disc);
	}
}
