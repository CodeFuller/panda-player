using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GalaSoft.MvvmLight.Messaging;
using PandaPlayer.DiscAdder.Events;

namespace PandaPlayer.DiscAdder.ViewModels.SourceContent
{
	internal class ActualDiscTreeItem : ActualBasicContentTreeItem
	{
		public override IEnumerable<ActualBasicContentTreeItem> ChildItems => Songs
			.Concat<ActualBasicContentTreeItem>(Enumerable.Repeat(new ActualDiscSeparatorTreeItem(), 1));

		public override string Title
		{
			get => DiscDirectory;
			set
			{
				DiscDirectory = value;
				RaisePropertyChanged();
			}
		}

		public override bool IsEditable => true;

		public IReadOnlyCollection<ActualSongTreeItem> Songs { get; }

		public IEnumerable<string> SongFileNames => Songs.Select(s => s.FilePath);

		private string discDirectory;

		public string DiscDirectory
		{
			get => discDirectory;
			set
			{
				if (value == discDirectory)
				{
					return;
				}

				var directoryMoved = false;

				if (discDirectory != null)
				{
					// Exception, thrown at this point, won't blow up,
					// because exceptions thrown from binding properties are treated as validation failures.
					// http://stackoverflow.com/questions/12658220/exceptions-thrown-during-a-set-operation-in-a-property-are-not-being-caught
					// http://stackoverflow.com/questions/1488472/best-practices-throwing-exceptions-from-properties
					// It's not a big problem because directory title will not be updated and still will be marked as incorrect.
					Directory.Move(discDirectory, value);
					directoryMoved = true;
				}

				discDirectory = value;

				if (directoryMoved)
				{
					OnDiscContentChanged();
				}
			}
		}

		private static void OnDiscContentChanged()
		{
			Messenger.Default.Send(new ActualContentChangedEventArgs());
		}

		private bool contentIsIncorrect;

		public bool ContentIsIncorrect
		{
			get => contentIsIncorrect;
			set => Set(ref contentIsIncorrect, value);
		}

		public ActualDiscTreeItem(ActualDiscContent disc)
		{
			_ = disc ?? throw new ArgumentNullException(nameof(disc));

			DiscDirectory = disc.DiscDirectory;

			Songs = disc.Songs.Select(x => new ActualSongTreeItem(this, x)).ToList();
		}

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
