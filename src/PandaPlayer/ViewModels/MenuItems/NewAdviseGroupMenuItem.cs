using System;
using System.Threading;
using System.Threading.Tasks;
using CodeFuller.Library.Wpf;

namespace PandaPlayer.ViewModels.MenuItems
{
	public class NewAdviseGroupMenuItem : NormalMenuItem
	{
		public NewAdviseGroupMenuItem(Func<CancellationToken, Task> commandAction)
		{
			Header = "New Advise Group ...";
			Command = new AsyncRelayCommand(() => commandAction(CancellationToken.None));
		}
	}
}
