using System.Threading.Tasks;

namespace CF.MusicLibrary.PandaPlayer.ViewModels.Interfaces
{
	public interface IDiscImageViewModel
	{
		string CurrImageFileName { get; }

		Task EditDiscImage();
	}
}
