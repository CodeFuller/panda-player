using System;
using System.Collections.Generic;
using System.Linq;

namespace PandaPlayer.DiscAdder.ViewModels.SourceContent
{
	internal class ReferenceSongTreeItem : ReferenceBasicTreeItem
	{
		private readonly SongContent song;

		public override IEnumerable<ReferenceBasicTreeItem> ChildItems => Enumerable.Empty<ReferenceBasicTreeItem>();

		public override string Title => song.Title;

		public ReferenceSongTreeItem(SongContent song)
		{
			this.song = song ?? throw new ArgumentNullException(nameof(song));
		}
	}
}
