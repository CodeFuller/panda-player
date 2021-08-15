using System.Windows.Input;

namespace PandaPlayer.ViewModels.MenuItems
{
	public abstract class NormalMenuItem : BasicMenuItem
	{
		public string Header { get; init; }

		public bool IsCheckable { get; init; }

		public bool IsChecked { get; init; }

		public ICommand Command { get; init; }
	}
}
