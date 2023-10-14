using System;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace PandaPlayer.ViewModels.MenuItems
{
	public class CommandMenuItem : VisibleMenuItem
	{
		private readonly ICommand command;

		public CommandMenuItem(Action commandAction)
		{
			command = new RelayCommand(commandAction);
		}

		public CommandMenuItem(Func<Task> commandAction)
		{
			command = new AsyncRelayCommand(commandAction);
		}

		public override MenuItem GetMenuItemControl()
		{
			var menuItem = base.GetMenuItemControl();
			menuItem.Command = command;

			return menuItem;
		}
	}
}
