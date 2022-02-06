using System;
using System.Collections.Generic;
using System.Linq;

namespace PandaPlayer.DiscAdder.ViewModels.SourceContent
{
	internal class ReferenceDiscSeparatorTreeItem : ReferenceBasicTreeItem
	{
		public override IEnumerable<ReferenceBasicTreeItem> ChildItems => Enumerable.Empty<ReferenceBasicTreeItem>();

		public override string Title => String.Empty;
	}
}
