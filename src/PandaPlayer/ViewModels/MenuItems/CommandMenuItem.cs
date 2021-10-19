using System.Windows.Controls;
using System.Windows.Input;

namespace PandaPlayer.ViewModels.MenuItems
{
	public class CommandMenuItem : VisibleMenuItem
	{
		public ICommand Command { get; init; }

		public override MenuItem GetMenuItemControl()
		{
			var menuItem = base.GetMenuItemControl();
			menuItem.Command = Command;

			return menuItem;
		}
	}
}
