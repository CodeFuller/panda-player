using System;
using System.Collections.Generic;
using System.Linq;

namespace PandaPlayer.DiscAdder.ViewModels.SourceContent
{
	internal class ReferenceSongTreeItem : ReferenceBasicTreeItem
	{
		private readonly ReferenceSongContent song;

		public string ExpectedTitle => song.ExpectedTitle;

		public string ExpectedTitleWithTrackNumber => song.ExpectedTitleWithTrackNumber;

		public override IEnumerable<ReferenceBasicTreeItem> ChildItems => Enumerable.Empty<ReferenceBasicTreeItem>();

		public override string Title => ExpectedTitle;

		public ReferenceSongTreeItem(ReferenceSongContent song)
		{
			this.song = song ?? throw new ArgumentNullException(nameof(song));
		}
	}
}
