using System;
using GalaSoft.MvvmLight.Messaging;
using PandaPlayer.Core.Models;
using PandaPlayer.Events.DiscEvents;

namespace PandaPlayer.ViewModels.LibraryExplorerItems
{
	public class DiscExplorerItem : BasicExplorerItem
	{
		public DiscModel Disc { get; }

		public ItemId DiscId => Disc.Id;

		public override string Title => Disc.TreeTitle;

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
			};
		}
	}
}
