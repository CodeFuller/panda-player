using System;
using System.Windows.Input;

namespace PandaPlayer.ViewModels.Interfaces
{
	public interface IDiscImageViewModel
	{
		Uri CurrentImageUri { get; }

		ICommand EditDiscImageCommand { get; }
	}
}
