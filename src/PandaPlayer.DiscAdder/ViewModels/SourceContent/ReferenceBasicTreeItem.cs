using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;

namespace PandaPlayer.DiscAdder.ViewModels.SourceContent
{
	internal abstract class ReferenceBasicTreeItem : ViewModelBase
	{
		public ObservableCollection<ReferenceBasicTreeItem> ChildItems { get; } = new();

		public abstract string ViewTitle { get; }

		private bool contentIsIncorrect;

		public bool ContentIsIncorrect
		{
			get => contentIsIncorrect;
			set => Set(ref contentIsIncorrect, value);
		}
	}
}
