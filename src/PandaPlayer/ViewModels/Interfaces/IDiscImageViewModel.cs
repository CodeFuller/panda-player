using System;

namespace PandaPlayer.ViewModels.Interfaces
{
	public interface IDiscImageViewModel
	{
		Uri CurrentImageUri { get; }

		void EditDiscImage();
	}
}
