using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using PandaPlayer.DiscAdder.ViewModels.Interfaces;
using PandaPlayer.Shared.Extensions;

namespace PandaPlayer.DiscAdder.ViewModels.SourceContent
{
	internal class ReferenceContentViewModel : IReferenceContentViewModel
	{
		public ObservableCollection<ReferenceDiscTreeItem> ExpectedDiscs { get; } = new();

		public bool ContentIsIncorrect => !ExpectedDiscs.Any() || ExpectedDiscs.Any(x => x.ContentIsIncorrect);

		public void SetExpectedDiscs(IEnumerable<ReferenceDiscContent> expectedDiscs)
		{
			var newExpectedDiscs = expectedDiscs.ToList();

			// We reload full ExpectedDiscs collection only when items count changes.
			// This operation causes visible UI delay and reload of control content.
			// Fortunately, most often this happens when reference content is reloaded.
			if (ExpectedDiscs.Count != newExpectedDiscs.Count)
			{
				ExpectedDiscs.Clear();
				ExpectedDiscs.AddRange(newExpectedDiscs.Select(x => new ReferenceDiscTreeItem(x)));
				return;
			}

			foreach (var (discTreeItem, newDiscContent) in ExpectedDiscs.Zip(newExpectedDiscs))
			{
				discTreeItem.Update(newDiscContent);
			}
		}
	}
}
