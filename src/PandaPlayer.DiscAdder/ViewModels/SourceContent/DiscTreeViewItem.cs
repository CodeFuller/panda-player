using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GalaSoft.MvvmLight.Messaging;
using PandaPlayer.DiscAdder.Events;

namespace PandaPlayer.DiscAdder.ViewModels.SourceContent
{
	internal class DiscTreeViewItem : BasicDiscTreeViewItem
	{
		private readonly IReadOnlyCollection<SongTreeViewItem> songItems;

		public override IEnumerable<BasicDiscTreeViewItem> ChildItems => songItems
			.Concat<BasicDiscTreeViewItem>(Enumerable.Repeat(new SeparatorLineViewItem(), 1));

		public override string Title
		{
			get => DiscDirectory;
			set
			{
				DiscDirectory = value;
				RaisePropertyChanged();
			}
		}

		public IEnumerable<SongTreeViewItem> Songs => songItems;

		public IEnumerable<string> SongFileNames => Songs.Select(s => s.FilePath);

		private string discDirectory;

		public string DiscDirectory
		{
			get => discDirectory;
			set
			{
				// Value has changed or just initialized?
				var valueChanged = discDirectory != null;

				if (valueChanged)
				{
					// Exception, thrown at this point, won't blow up,
					// because exceptions thrown from binding properties are treated as validation failures.
					// http://stackoverflow.com/questions/12658220/exceptions-thrown-during-a-set-operation-in-a-property-are-not-being-caught
					// http://stackoverflow.com/questions/1488472/best-practices-throwing-exceptions-from-properties
					// It's not a big problem because directory title will not be updated and still will be marked as incorrect.
					Directory.Move(discDirectory, value);
				}

				Set(ref discDirectory, value);

				if (valueChanged)
				{
					OnDiscContentChanged();
				}
			}
		}

		private static void OnDiscContentChanged()
		{
			Messenger.Default.Send(new DiscContentChangedEventArgs());
		}

		private bool contentIsIncorrect;

		public bool ContentIsIncorrect
		{
			get => contentIsIncorrect;
			set => Set(ref contentIsIncorrect, value);
		}

		public DiscTreeViewItem(DiscContent disc)
		{
			if (disc == null)
			{
				throw new ArgumentNullException(nameof(disc));
			}

			DiscDirectory = disc.DiscDirectory;

			songItems = disc.Songs.Select(x => new SongTreeViewItem(this, x)).ToList();
		}
	}
}
