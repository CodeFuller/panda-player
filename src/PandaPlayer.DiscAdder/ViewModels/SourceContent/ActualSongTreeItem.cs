using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommunityToolkit.Mvvm.Messaging;
using PandaPlayer.DiscAdder.Events;

namespace PandaPlayer.DiscAdder.ViewModels.SourceContent
{
	internal class ActualSongTreeItem : ActualBasicContentTreeItem
	{
		private readonly ActualDiscTreeItem discItem;

		private readonly IMessenger messenger;

		public override IEnumerable<ActualBasicContentTreeItem> ChildItems => Enumerable.Empty<ActualBasicContentTreeItem>();

		public override string ViewTitle
		{
			get => FileName;
			set
			{
				FileName = value;
				OnPropertyChanged();
			}
		}

		private string fileName;

		public string FileName
		{
			get => fileName;
			set
			{
				if (value == fileName)
				{
					return;
				}

				var fileMoved = false;

				if (fileName != null)
				{
					// Exception, thrown during file renaming, won't blow up,
					// because exceptions thrown from binding properties are treated as validation failures.
					// http://stackoverflow.com/questions/12658220/exceptions-thrown-during-a-set-operation-in-a-property-are-not-being-caught
					// http://stackoverflow.com/questions/1488472/best-practices-throwing-exceptions-from-properties
					// It's not a big problem because song file name will not be updated and still will be marked as incorrect.
					File.Move(GetFilePath(fileName), GetFilePath(value));
					fileMoved = true;
				}

				fileName = value;

				if (fileMoved)
				{
					messenger.Send(new ActualContentChangedEventArgs());
				}
			}
		}

		public override bool IsEditable => true;

		public string FilePath => GetFilePath(FileName);

		private bool contentIsIncorrect;

		public bool ContentIsIncorrect
		{
			get => contentIsIncorrect;
			set => SetProperty(ref contentIsIncorrect, value);
		}

		public ActualSongTreeItem(ActualDiscTreeItem discItem, ActualSongContent song, IMessenger messenger)
		{
			_ = song ?? throw new ArgumentNullException(nameof(song));
			this.messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));

			this.discItem = discItem;
			this.fileName = song.FileName;
		}

		private string GetFilePath(string songFileName)
		{
			return Path.Combine(discItem.DiscDirectory, songFileName);
		}
	}
}
