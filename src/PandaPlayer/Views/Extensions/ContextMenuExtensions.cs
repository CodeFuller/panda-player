using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using PandaPlayer.ViewModels.MenuItems;

namespace PandaPlayer.Views.Extensions
{
	public static class ContextMenuExtensions
	{
		public static ContextMenu ToContextMenu(this IEnumerable<BasicMenuItem> menuItems)
		{
			var contextMenu = new ContextMenu();
			foreach (var menuItem in menuItems.Select(x => x.GetMenuItemControl()))
			{
				contextMenu.Items.Add(menuItem);
			}

			return contextMenu;
		}
	}
}
