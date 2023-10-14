using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace PandaPlayer.DiscAdder.ViewModels.SourceContent
{
	internal abstract class ReferenceBasicTreeItem : ObservableObject
	{
		public ObservableCollection<ReferenceBasicTreeItem> ChildItems { get; } = new();

		public abstract string ViewTitle { get; }

		private bool contentIsIncorrect;

		public bool ContentIsIncorrect
		{
			get => contentIsIncorrect;
			set => SetProperty(ref contentIsIncorrect, value);
		}
	}
}
