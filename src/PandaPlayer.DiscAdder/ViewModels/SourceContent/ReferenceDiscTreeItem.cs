using System.Collections.Generic;
using System.Linq;
using PandaPlayer.Shared.Extensions;

namespace PandaPlayer.DiscAdder.ViewModels.SourceContent
{
	internal class ReferenceDiscTreeItem : ReferenceBasicTreeItem
	{
		private string expectedDirectoryPath;

		public string ExpectedDirectoryPath
		{
			get => expectedDirectoryPath;
			set
			{
				Set(ref expectedDirectoryPath, value);
				RaisePropertyChanged(nameof(ViewTitle));
			}
		}

		public IReadOnlyCollection<ReferenceSongTreeItem> ExpectedSongs { get; private set; }

		public override string ViewTitle => ExpectedDirectoryPath;

		public ReferenceDiscTreeItem(ReferenceDiscContent disc)
		{
			ExpectedDirectoryPath = disc.ExpectedDirectoryPath;

			InitializeExpectedSongs(disc);
		}

		public void Update(ReferenceDiscContent newDiscContent)
		{
			ExpectedDirectoryPath = newDiscContent.ExpectedDirectoryPath;

			if (ExpectedSongs.Count != newDiscContent.ExpectedSongs.Count)
			{
				InitializeExpectedSongs(newDiscContent);
				return;
			}

			foreach (var (songTreeItem, newSongContent) in ExpectedSongs.Zip(newDiscContent.ExpectedSongs))
			{
				songTreeItem.Update(newSongContent);
			}
		}

		private void InitializeExpectedSongs(ReferenceDiscContent disc)
		{
			ExpectedSongs = disc.ExpectedSongs.Select(x => new ReferenceSongTreeItem(x)).ToList();

			ChildItems.Clear();
			ChildItems.AddRange(ExpectedSongs);
			ChildItems.Add(new ReferenceDiscSeparatorTreeItem());
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
