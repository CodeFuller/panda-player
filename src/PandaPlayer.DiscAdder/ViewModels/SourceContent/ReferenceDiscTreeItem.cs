using System;
using System.Collections.Generic;
using System.Linq;

namespace PandaPlayer.DiscAdder.ViewModels.SourceContent
{
	internal class ReferenceDiscTreeItem : ReferenceBasicTreeItem
	{
		private readonly DiscContent disc;

		public IReadOnlyCollection<ReferenceSongTreeItem> Songs { get; }

		public override IEnumerable<ReferenceBasicTreeItem> ChildItems => Songs
			.Concat<ReferenceBasicTreeItem>(Enumerable.Repeat(new ReferenceDiscSeparatorTreeItem(), 1));

		public override string Title => disc.DiscDirectory;

		public string DiscDirectory { get; }

		public ReferenceDiscTreeItem(DiscContent disc)
		{
			this.disc = disc ?? throw new ArgumentNullException(nameof(disc));

			DiscDirectory = disc.DiscDirectory;

			Songs = disc.Songs.Select(x => new ReferenceSongTreeItem(x)).ToList();
		}

		// TODO: Remove duplication with ActualDiscTreeItem.
		public void MarkWholeDiscAsIncorrect()
		{
			foreach (var song in Songs)
			{
				song.ContentIsIncorrect = true;
			}

			ContentIsIncorrect = true;
		}
	}
}
