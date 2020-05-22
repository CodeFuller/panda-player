using System;
using System.Threading.Tasks;

namespace MusicLibrary.PandaPlayer.ViewModels.Interfaces
{
	public interface IDiscImageViewModel
	{
		Uri CurrentImageUri { get; }

		Task EditDiscImage();
	}
}
