using System;
using System.Threading;
using System.Threading.Tasks;
using CodeFuller.Library.Wpf;
using PandaPlayer.Core.Models;

namespace PandaPlayer.ViewModels.MenuItems
{
	public class ReverseFavoriteStatusForAdviseGroupMenuItem : NormalMenuItem
	{
		public ReverseFavoriteStatusForAdviseGroupMenuItem(AdviseGroupModel adviseGroup, Func<AdviseGroupModel, CancellationToken, Task> commandAction)
		{
			Header = $"{(adviseGroup.IsFavorite ? "Unmark" : "Mark")} '{adviseGroup.Name}' as favorite";
			Command = new AsyncRelayCommand(() => commandAction(adviseGroup, CancellationToken.None));
		}
	}
}
