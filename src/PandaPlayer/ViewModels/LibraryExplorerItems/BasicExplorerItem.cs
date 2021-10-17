using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight;
using MaterialDesignThemes.Wpf;
using PandaPlayer.ViewModels.AdviseGroups;
using PandaPlayer.ViewModels.Interfaces;
using PandaPlayer.ViewModels.MenuItems;

namespace PandaPlayer.ViewModels.LibraryExplorerItems
{
	public abstract class BasicExplorerItem : ViewModelBase
	{
		public abstract string Title { get; }

		public abstract PackIconKind IconKind { get; }

		public abstract bool IsDeleted { get; }

		public abstract IEnumerable<BasicMenuItem> GetContextMenuItems(ILibraryExplorerViewModel libraryExplorerViewModel, IAdviseGroupHelper adviseGroupHelper);

		protected static IReadOnlyCollection<BasicMenuItem> GetAdviseGroupMenuItems(BasicAdviseGroupHolder adviseGroupHolder, ILibraryExplorerViewModel libraryExplorerViewModel, IAdviseGroupHelper adviseGroupHelper)
		{
			var menuItems = new List<BasicMenuItem>
			{
				new NewAdviseGroupMenuItem(ct => libraryExplorerViewModel.CreateAdviseGroup(adviseGroupHolder, ct)),
			};

			if (adviseGroupHolder.CurrentAdviseGroup != null)
			{
				var reverseFavoriteMenuItem = new ReverseFavoriteStatusForAdviseGroupMenuItem(
					adviseGroupHolder.CurrentAdviseGroup, adviseGroupHelper.ReverseFavoriteStatus);

				menuItems.Add(reverseFavoriteMenuItem);
			}

			var adviseGroups = adviseGroupHelper.AdviseGroups;
			if (adviseGroups.Any())
			{
				menuItems.Add(new SeparatorMenuItem());

				var currentAdviseGroupId = adviseGroupHolder.CurrentAdviseGroup?.Id;
				menuItems.AddRange(adviseGroups.Select(x => new SetAdviseGroupMenuItem(x, x.Id == currentAdviseGroupId, ct => adviseGroupHelper.ReverseAdviseGroup(adviseGroupHolder, x, ct))));
			}

			return menuItems;
		}
	}
}
