using System;
using System.Threading;
using System.Threading.Tasks;
using CodeFuller.Library.Wpf;
using MaterialDesignThemes.Wpf;
using PandaPlayer.Core.Models;

namespace PandaPlayer.ViewModels.MenuItems
{
	public class SetAdviseGroupMenuItem : CommandMenuItem
	{
		public SetAdviseGroupMenuItem(AdviseGroupModel adviseGroup, bool isAssignedAdviseGroup, Func<CancellationToken, Task> commandAction)
		{
			Header = adviseGroup.Name;
			IconKind = GetIconKind(adviseGroup, isAssignedAdviseGroup);
			Command = new AsyncRelayCommand(() => commandAction(CancellationToken.None));
		}

		private static PackIconKind? GetIconKind(AdviseGroupModel adviseGroup, bool isAssignedAdviseGroup)
		{
			if (isAssignedAdviseGroup)
			{
				// We do not use checkable menu items, because there is no easy way to have checkable item with an icon (https://stackoverflow.com/questions/3842493).
				return PackIconKind.Check;
			}

			if (adviseGroup.IsFavorite)
			{
				return PackIconKind.Heart;
			}

			return null;
		}
	}
}
