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

		public bool ContentIsIncorrect => ExpectedDiscs.Any(x => x.ContentIsIncorrect);

		public void SetExpectedDiscs(IEnumerable<ReferenceDiscContent> expectedDiscs)
		{
			ExpectedDiscs.Clear();
			ExpectedDiscs.AddRange(expectedDiscs.Select(x => new ReferenceDiscTreeItem(x)));
		}
	}
}
