using System.Windows.Controls;
using MaterialDesignThemes.Wpf;

namespace PandaPlayer.ViewModels.MenuItems
{
	public class VisibleMenuItem : BasicMenuItem
	{
		public string Header { get; init; }

		public PackIconKind? IconKind { get; init; }

		public override MenuItem MenuItemControl => new()
		{
			Header = Header,
			Icon = IconKind != null ? new PackIcon { Kind = IconKind.Value } : null,
		};
	}
}
