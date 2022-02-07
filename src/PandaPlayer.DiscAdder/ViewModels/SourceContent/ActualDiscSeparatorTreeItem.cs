using System;
using System.Collections.Generic;
using System.Linq;

namespace PandaPlayer.DiscAdder.ViewModels.SourceContent
{
	internal class ActualDiscSeparatorTreeItem : ActualBasicContentTreeItem
	{
		public override IEnumerable<ActualBasicContentTreeItem> ChildItems => Enumerable.Empty<ActualDiscSeparatorTreeItem>();

		public override string ViewTitle
		{
			get => String.Empty;
			set => throw new InvalidOperationException($"{nameof(ViewTitle)} property is read-only");
		}

		public override bool IsEditable => false;
	}
}
