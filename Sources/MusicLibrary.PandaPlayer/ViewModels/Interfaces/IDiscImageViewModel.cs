using System.Threading.Tasks;

namespace MusicLibrary.PandaPlayer.ViewModels.Interfaces
{
	public interface IDiscImageViewModel
	{
		string CurrImageFileName { get; }

		Task EditDiscImage();
	}
}
