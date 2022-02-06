using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using PandaPlayer.DiscAdder.ViewModels.Interfaces;
using PandaPlayer.Shared.Extensions;

namespace PandaPlayer.DiscAdder.ViewModels.SourceContent
{
	internal class ActualContentViewModel : IActualContentViewModel
	{
		public ObservableCollection<ActualDiscTreeItem> Discs { get; } = new();

		public bool ContentIsIncorrect => Discs.Any(x => x.ContentIsIncorrect);

		public void SetContent(IEnumerable<ActualDiscContent> discs)
		{
			Discs.Clear();
			Discs.AddRange(discs.Select(x => new ActualDiscTreeItem(x)));
		}
	}
}
