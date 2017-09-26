using System.Threading.Tasks;

namespace CF.MusicLibrary.PandaPlayer.ViewModels.Interfaces
{
	public interface IDiscArtViewModel
	{
		string CurrImageFileName { get; }

		Task EditDiscArt();
	}
}
