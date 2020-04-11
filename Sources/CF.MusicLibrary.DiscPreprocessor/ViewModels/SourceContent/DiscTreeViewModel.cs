using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CF.Library.Core.Extensions;
using GalaSoft.MvvmLight;

namespace CF.MusicLibrary.DiscPreprocessor.ViewModels.SourceContent
{
#pragma warning disable CA1710 // Identifiers should have correct suffix - Following ViewModel naming convention
	public class DiscTreeViewModel : ViewModelBase, IEnumerable<DiscTreeViewItem>
#pragma warning restore CA1710 // Identifiers should have correct suffix
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
