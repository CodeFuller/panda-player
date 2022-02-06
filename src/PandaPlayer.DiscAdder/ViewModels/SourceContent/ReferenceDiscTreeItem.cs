using System;
using System.Collections.Generic;
using System.Linq;

namespace PandaPlayer.DiscAdder.ViewModels.SourceContent
{
	internal class ReferenceDiscTreeItem : ReferenceBasicTreeItem
	{
		private readonly ReferenceDiscContent disc;

		public string ExpectedDirectoryPath => disc.ExpectedDirectoryPath;

		public IReadOnlyCollection<ReferenceSongTreeItem> ExpectedSongs { get; }

		public override IEnumerable<ReferenceBasicTreeItem> ChildItems => ExpectedSongs
			.Concat<ReferenceBasicTreeItem>(Enumerable.Repeat(new ReferenceDiscSeparatorTreeItem(), 1));

		public override string Title => ExpectedDirectoryPath;

		public ReferenceDiscTreeItem(ReferenceDiscContent disc)
		{
			this.disc = disc ?? throw new ArgumentNullException(nameof(disc));

			ExpectedSongs = disc.ExpectedSongs.Select(x => new ReferenceSongTreeItem(x)).ToList();
		}

		public void MarkWholeDiscAsIncorrect()
		{
			foreach (var song in ExpectedSongs)
			{
				song.ContentIsIncorrect = true;
			}

			ContentIsIncorrect = true;
		}
	}
}
