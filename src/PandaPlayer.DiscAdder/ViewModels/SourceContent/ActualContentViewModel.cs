using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.Messaging;
using PandaPlayer.DiscAdder.ViewModels.Interfaces;
using PandaPlayer.Shared.Extensions;

namespace PandaPlayer.DiscAdder.ViewModels.SourceContent
{
	internal class ActualContentViewModel : IActualContentViewModel
	{
		private readonly IMessenger messenger;

		public ObservableCollection<ActualDiscTreeItem> Discs { get; } = new();

		public bool ContentIsIncorrect => !Discs.Any() || Discs.Any(x => x.ContentIsIncorrect);

		public ActualContentViewModel(IMessenger messenger)
		{
			this.messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));
		}

		public void SetContent(IEnumerable<ActualDiscContent> discs)
		{
			Discs.Clear();
			Discs.AddRange(discs.Select(x => new ActualDiscTreeItem(x, messenger)));
		}
	}
}
