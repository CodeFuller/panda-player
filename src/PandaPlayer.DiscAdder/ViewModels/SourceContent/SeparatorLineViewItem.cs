using System;
using System.Collections.Generic;
using System.Linq;

namespace PandaPlayer.DiscAdder.ViewModels.SourceContent
{
	internal class SeparatorLineViewItem : BasicDiscTreeViewItem
	{
		public override IEnumerable<BasicDiscTreeViewItem> ChildItems => Enumerable.Empty<BasicDiscTreeViewItem>();

		public override string Title
		{
			get => String.Empty;
			set
			{
			}
		}
	}
}
