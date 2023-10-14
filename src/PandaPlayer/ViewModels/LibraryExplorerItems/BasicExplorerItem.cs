using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using MaterialDesignThemes.Wpf;
using PandaPlayer.Core.Models;
using PandaPlayer.ViewModels.AdviseGroups;
using PandaPlayer.ViewModels.Interfaces;
using PandaPlayer.ViewModels.MenuItems;

namespace PandaPlayer.ViewModels.LibraryExplorerItems
{
	public abstract class BasicExplorerItem : ObservableObject
	{
		public abstract string Title { get; }

		public abstract PackIconKind IconKind { get; }

		public abstract bool IsDeleted { get; }

		public virtual string ToolTip => null;

		public abstract IEnumerable<BasicMenuItem> GetContextMenuItems(ILibraryExplorerViewModel libraryExplorerViewModel, IAdviseGroupHelper adviseGroupHelper);

		protected static IReadOnlyCollection<BasicMenuItem> GetAdviseGroupMenuItems(BasicAdviseGroupHolder adviseGroupHolder, ILibraryExplorerViewModel libraryExplorerViewModel, IAdviseGroupHelper adviseGroupHelper)
		{
			var menuItems = new List<BasicMenuItem>
			{
				new CommandMenuItem(() => libraryExplorerViewModel.CreateAdviseGroup(adviseGroupHolder, CancellationToken.None))
				{
					Header = "New Advise Group ...",
					IconKind = PackIconKind.FolderPlus,
				},
			};

			var currentAdviseGroup = adviseGroupHolder.CurrentAdviseGroup;
			if (currentAdviseGroup != null)
			{
				var reverseFavoriteMenuItem = new CommandMenuItem(() => adviseGroupHelper.ReverseFavoriteStatus(currentAdviseGroup, CancellationToken.None))
				{
					Header = $"{(currentAdviseGroup.IsFavorite ? "Unmark" : "Mark")} '{currentAdviseGroup.Name}' as favorite",
					IconKind = currentAdviseGroup.IsFavorite ? PackIconKind.HeartBroken : PackIconKind.Heart,
				};

				menuItems.Add(reverseFavoriteMenuItem);
			}

			var adviseGroups = adviseGroupHelper.AdviseGroups;
			if (adviseGroups.Any())
			{
				menuItems.Add(new SeparatorMenuItem());

				menuItems.AddRange(adviseGroups.Select(x => CreateSetAdviseGroupMenuItem(adviseGroupHolder, adviseGroupHelper, x)));
			}

			return menuItems;
		}

		private static CommandMenuItem CreateSetAdviseGroupMenuItem(BasicAdviseGroupHolder adviseGroupHolder, IAdviseGroupHelper adviseGroupHelper, AdviseGroupModel adviseGroup)
		{
			PackIconKind? iconKind = null;

			if (adviseGroup.Id == adviseGroupHolder.CurrentAdviseGroup?.Id)
			{
				// We do not use checkable menu items, because there is no easy way to have checkable item with an icon (https://stackoverflow.com/questions/3842493).
				iconKind = PackIconKind.Check;
			}

			if (adviseGroup.IsFavorite)
			{
				iconKind = PackIconKind.Heart;
			}

			return new CommandMenuItem(() => adviseGroupHelper.ReverseAdviseGroup(adviseGroupHolder, adviseGroup, CancellationToken.None))
			{
				Header = adviseGroup.Name,
				IconKind = iconKind,
			};
		}
	}
}
