using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using GalaSoft.MvvmLight.Messaging;
using MaterialDesignThemes.Wpf;
using PandaPlayer.Core.Extensions;
using PandaPlayer.Core.Models;
using PandaPlayer.Events.DiscEvents;
using PandaPlayer.ViewModels.AdviseGroups;
using PandaPlayer.ViewModels.Interfaces;
using PandaPlayer.ViewModels.MenuItems;

namespace PandaPlayer.ViewModels.LibraryExplorerItems
{
	public class DiscExplorerItem : BasicExplorerItem
	{
		public DiscModel Disc { get; }

		public ItemId DiscId => Disc.Id;

		public override string Title => Disc.TreeTitle;

		public override PackIconKind IconKind => Disc.AdviseGroup != null || Disc.AdviseSetInfo != null ? PackIconKind.DiscAlert : PackIconKind.Album;

		public override bool IsDeleted => Disc.IsDeleted;

		public override string ToolTip
		{
			get
			{
				if (!IsDeleted)
				{
					return null;
				}

				var deleteDate = Disc.AllSongs
					.Select(x => x.DeleteDate?.Date)
					.UniqueOrDefault();

				var deleteComments = Disc.AllSongs
					.Select(x => x.DeleteComment)
					.Where(x => !String.IsNullOrWhiteSpace(x))
					.Distinct()
					.ToList();

				if (deleteDate != null)
				{
					return deleteComments.Count switch
					{
						0 => $"The disc was deleted on {deleteDate:yyyy.MM.dd} without comment",
						1 => $"The disc was deleted on {deleteDate:yyyy.MM.dd} with the comment '{deleteComments.Single()}'",
						_ => $"The disc was deleted on {deleteDate:yyyy.MM.dd} with various comments",
					};
				}

				return deleteComments.Count switch
				{
					0 => "The disc was deleted without comment",
					1 => $"The disc was deleted with the comment '{deleteComments.Single()}'",
					_ => "The disc was deleted with various comments",
				};
			}
		}

		public DiscExplorerItem(DiscModel disc)
		{
			this.Disc = disc ?? throw new ArgumentNullException(nameof(disc));

			Disc.PropertyChanged += (_, args) =>
			{
				Messenger.Default.Send(new DiscChangedEventArgs(Disc, args.PropertyName));

				if (args.PropertyName == nameof(Disc.TreeTitle))
				{
					RaisePropertyChanged(nameof(Title));
				}

				if (args.PropertyName == nameof(Disc.AdviseGroup))
				{
					RaisePropertyChanged(nameof(IconKind));
				}
			};
		}

		public override IEnumerable<BasicMenuItem> GetContextMenuItems(ILibraryExplorerViewModel libraryExplorerViewModel, IAdviseGroupHelper adviseGroupHelper)
		{
			return Disc.IsDeleted
				? GetContextMenuItemsForDeletedDisc(libraryExplorerViewModel)
				: GetContextMenuItemsForActiveDisc(libraryExplorerViewModel, adviseGroupHelper);
		}

		private IEnumerable<BasicMenuItem> GetContextMenuItemsForActiveDisc(ILibraryExplorerViewModel libraryExplorerViewModel, IAdviseGroupHelper adviseGroupHelper)
		{
			yield return new ExpandableMenuItem
			{
				Header = "Set Advise Group",
				IconKind = PackIconKind.FolderStar,
				Items = GetAdviseGroupMenuItems(new DiscAdviseGroupHolder(Disc), libraryExplorerViewModel, adviseGroupHelper),
			};

			yield return new CommandMenuItem(() => libraryExplorerViewModel.PlayDisc(Disc), keepTargetAlive: true)
			{
				Header = "Play Disc",
				IconKind = PackIconKind.Play,
			};

			yield return new CommandMenuItem(() => libraryExplorerViewModel.AddDiscToPlaylist(Disc), keepTargetAlive: true)
			{
				Header = "Add To Playlist",
				IconKind = PackIconKind.PlaylistPlus,
			};

			yield return new CommandMenuItem(() => libraryExplorerViewModel.DeleteDisc(Disc, CancellationToken.None))
			{
				Header = "Delete Disc",
				IconKind = PackIconKind.DeleteForever,
			};

			yield return new CommandMenuItem(() => libraryExplorerViewModel.EditDiscProperties(Disc), keepTargetAlive: true)
			{
				Header = "Properties",
				IconKind = PackIconKind.Pencil,
			};
		}

		private IEnumerable<BasicMenuItem> GetContextMenuItemsForDeletedDisc(ILibraryExplorerViewModel libraryExplorerViewModel)
		{
			yield return new CommandMenuItem(() => libraryExplorerViewModel.EditDiscProperties(Disc), keepTargetAlive: true)
			{
				Header = "Properties",
				IconKind = PackIconKind.Pencil,
			};
		}
	}
}
