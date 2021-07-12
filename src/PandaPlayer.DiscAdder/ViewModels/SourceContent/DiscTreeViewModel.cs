using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
using PandaPlayer.Shared.Extensions;

namespace PandaPlayer.DiscAdder.ViewModels.SourceContent
{
	internal class DiscTreeViewModel : ViewModelBase, IEnumerable<DiscTreeViewItem>
	{
		public ObservableCollection<DiscTreeViewItem> Discs { get; } = new ObservableCollection<DiscTreeViewItem>();

		public bool ContentIsIncorrect => Discs.Any(s => s.ContentIsIncorrect);

		public void SetDiscs(IEnumerable<DiscContent> newDiscs)
		{
			Discs.Clear();
			Discs.AddRange(newDiscs.Select(a => new DiscTreeViewItem(a)));
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public IEnumerator<DiscTreeViewItem> GetEnumerator()
		{
			return Discs.GetEnumerator();
		}
	}
}
