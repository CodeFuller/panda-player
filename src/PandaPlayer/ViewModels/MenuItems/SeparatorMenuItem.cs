using System.Windows.Controls;

namespace PandaPlayer.ViewModels.MenuItems
{
	public class SeparatorMenuItem : BasicMenuItem
	{
		// We have this property only to make Fluent Assertions happy and avoid exception "No members were found for comparison".
#pragma warning disable CA1822 // Mark members as static
		public bool Test => false;
#pragma warning restore CA1822 // Mark members as static

		public override Control GetMenuItemControl()
		{
			return new Separator();
		}
	}
}
