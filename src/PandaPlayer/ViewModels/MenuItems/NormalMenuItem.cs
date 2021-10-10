using System.Windows.Input;
using MaterialDesignThemes.Wpf;

namespace PandaPlayer.ViewModels.MenuItems
{
	public abstract class NormalMenuItem : BasicMenuItem
	{
		public string Header { get; init; }

		public PackIconKind? IconKind { get; init; }

		public ICommand Command { get; init; }
	}
}
