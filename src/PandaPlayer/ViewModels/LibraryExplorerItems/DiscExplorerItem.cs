using System;
using System.Collections.Generic;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MaterialDesignThemes.Wpf;
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
			if (Disc.IsDeleted)
			{
				yield break;
			}

			yield return new ExpandableMenuItem
			{
				Header = "Set Advise Group",
				IconKind = PackIconKind.FolderStar,
				Items = GetAdviseGroupMenuItems(new DiscAdviseGroupHolder(Disc), libraryExplorerViewModel, adviseGroupHelper),
			};

			yield return new CommandMenuItem
			{
				Header = "Play Disc",
				IconKind = PackIconKind.Play,
				Command = new RelayCommand(() => libraryExplorerViewModel.PlayDisc(Disc), keepTargetAlive: true),
			};

			yield return new CommandMenuItem
			{
				Header = "Add To Playlist",
				IconKind = PackIconKind.PlaylistPlus,
				Command = new RelayCommand(() => libraryExplorerViewModel.AddDiscToPlaylist(Disc), keepTargetAlive: true),
			};

			yield return new CommandMenuItem
			{
				Header = "Delete Disc",
				IconKind = PackIconKind.DeleteForever,
				Command = new RelayCommand(() => libraryExplorerViewModel.DeleteDisc(Disc), keepTargetAlive: true),
			};

			yield return new CommandMenuItem
			{
				Header = "Properties",
				IconKind = PackIconKind.Pencil,
				Command = new RelayCommand(() => libraryExplorerViewModel.EditDiscProperties(Disc), keepTargetAlive: true),
			};
		}
	}
}
