using System.Collections.Generic;
using System.Collections.ObjectModel;
using PandaPlayer.DiscAdder.ViewModels.SourceContent;

namespace PandaPlayer.DiscAdder.ViewModels.Interfaces
{
	internal interface IActualContentViewModel
	{
		ObservableCollection<ActualDiscTreeItem> Discs { get; }

		bool ContentIsIncorrect { get; }

		void SetContent(IEnumerable<DiscContent> discs);
	}
}
