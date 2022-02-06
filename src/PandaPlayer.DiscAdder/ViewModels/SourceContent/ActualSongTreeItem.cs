using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GalaSoft.MvvmLight.Messaging;
using PandaPlayer.DiscAdder.Events;

namespace PandaPlayer.DiscAdder.ViewModels.SourceContent
{
	internal class ActualSongTreeItem : ActualBasicContentTreeItem
	{
		private readonly ActualDiscTreeItem discItem;

		public override IEnumerable<ActualBasicContentTreeItem> ChildItems => Enumerable.Empty<ActualBasicContentTreeItem>();

		private string title;

		public override string Title
		{
			get => title;
			set
			{
				// Value has changed or just initialized?
				var valueChanged = title != null;

				if (valueChanged)
				{
					// Exception, thrown during file renaming, won't blow up,
					// because exceptions thrown from binding properties are treated as validation failures.
					// http://stackoverflow.com/questions/12658220/exceptions-thrown-during-a-set-operation-in-a-property-are-not-being-caught
					// http://stackoverflow.com/questions/1488472/best-practices-throwing-exceptions-from-properties
					// It's not a big problem because song title will not be updated and still will be marked as incorrect.
					File.Move(GetFilePath(title), GetFilePath(value));
				}

				Set(ref title, value);

				if (valueChanged)
				{
					Messenger.Default.Send(new ActualContentChangedEventArgs());
				}
			}
		}

		public override bool IsEditable => true;

		public string FilePath => GetFilePath(Title);

		private bool contentIsIncorrect;

		public bool ContentIsIncorrect
		{
			get => contentIsIncorrect;
			set => Set(ref contentIsIncorrect, value);
		}

		public ActualSongTreeItem(ActualDiscTreeItem discItem, SongContent song)
		{
			_ = song ?? throw new ArgumentNullException(nameof(song));

			this.discItem = discItem;
			title = song.Title;
		}

		private string GetFilePath(string fileName)
		{
			return Path.Combine(discItem.DiscDirectory, fileName);
		}
	}
}
