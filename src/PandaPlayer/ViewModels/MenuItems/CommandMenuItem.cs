using System;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using CodeFuller.Library.Wpf;
using GalaSoft.MvvmLight.Command;

namespace PandaPlayer.ViewModels.MenuItems
{
	public class CommandMenuItem : VisibleMenuItem
	{
		private readonly ICommand command;

		public CommandMenuItem(Action commandAction, bool keepTargetAlive)
		{
			command = new RelayCommand(commandAction, keepTargetAlive);
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
