using System;

namespace MusicLibrary.PandaPlayer.ViewModels.Interfaces
{
	public interface IDiscImageViewModel
	{
		Uri CurrentImageUri { get; }

		void EditDiscImage();
	}
}
