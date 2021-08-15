using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CodeFuller.Library.Wpf;

namespace PandaPlayer.ViewModels.MenuItems
{
	public class SetAdviseGroupMenuItem
	{
		public string Title { get; }

		// This property must have setter, otherwise exception is thrown: "A TwoWay or OneWayToSource binding cannot work on the read-only property".
		public bool IsAssignedAdviseGroup { get; set; }

		public ICommand Command { get; }

		public SetAdviseGroupMenuItem(string title, bool isAssignedAdviseGroup, Func<CancellationToken, Task> commandAction)
		{
			Title = title;
			IsAssignedAdviseGroup = isAssignedAdviseGroup;
			Command = new AsyncRelayCommand(() => commandAction(CancellationToken.None));
		}
	}
}
