using System.Windows.Input;
using PandaPlayer.ViewModels.DiscImages;

namespace PandaPlayer.ViewModels.Interfaces
{
	public interface IDiscImageViewModel
	{
		DiscImageSource CoverImageSource { get; }

		ICommand EditDiscImageCommand { get; }
	}
}
