using System.Collections.Generic;
using GalaSoft.MvvmLight;

namespace PandaPlayer.DiscAdder.ViewModels.SourceContent
{
	internal abstract class ReferenceBasicTreeItem : ViewModelBase
	{
		public abstract IEnumerable<ReferenceBasicTreeItem> ChildItems { get; }

		public abstract string Title { get; }

		private bool contentIsIncorrect;

		public bool ContentIsIncorrect
		{
			get => contentIsIncorrect;
			set => Set(ref contentIsIncorrect, value);
		}
	}
}
