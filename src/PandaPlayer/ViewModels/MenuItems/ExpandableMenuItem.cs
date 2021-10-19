using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace PandaPlayer.ViewModels.MenuItems
{
	public class ExpandableMenuItem : VisibleMenuItem
	{
		public IEnumerable<BasicMenuItem> Items { get; init; }

		public override MenuItem GetMenuItemControl()
		{
			var menuItem = base.GetMenuItemControl();

			foreach (var subItem in Items.Select(x => x.GetMenuItemControl()))
			{
				menuItem.Items.Add(subItem);
			}

			return menuItem;
		}
	}
}
