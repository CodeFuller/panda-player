using System.Windows.Controls;

namespace PandaPlayer.ViewModels.MenuItems
{
	public class SeparatorMenuItem : BasicMenuItem
	{
		public override Control MenuItemControl => new Separator();
	}
}
