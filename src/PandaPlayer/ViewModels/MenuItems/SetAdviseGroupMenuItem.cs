using System;
using System.Threading;
using System.Threading.Tasks;
using CodeFuller.Library.Wpf;

namespace PandaPlayer.ViewModels.MenuItems
{
	public class SetAdviseGroupMenuItem : NormalMenuItem
	{
		public SetAdviseGroupMenuItem(string adviseGroupName, bool isAssignedAdviseGroup, Func<CancellationToken, Task> commandAction)
		{
			Header = adviseGroupName;
			IsCheckable = true;
			IsChecked = isAssignedAdviseGroup;
			Command = new AsyncRelayCommand(() => commandAction(CancellationToken.None));
		}
	}
}
