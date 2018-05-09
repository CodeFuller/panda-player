using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CF.Library.Core.Extensions;
using GalaSoft.MvvmLight;

namespace CF.MusicLibrary.DiscPreprocessor.ViewModels.SourceContent
{
	public class DiscTreeViewModel : ViewModelBase, IEnumerable<DiscTreeViewItem>
	{
		private Collection<DiscTreeViewItem> discs;
		public Collection<DiscTreeViewItem> Discs
		{
			get => discs;
			set => Set(ref discs, value);
		}

		public bool ContentIsIncorrect => Discs.Any(s => s.ContentIsIncorrect);

		public DiscTreeViewModel()
		{
			Discs = new Collection<DiscTreeViewItem>();
		}

		public void SetDiscs(IEnumerable<DiscContent> newDiscs)
		{
			Discs = newDiscs.Select(a => new DiscTreeViewItem(a)).ToCollection();
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
