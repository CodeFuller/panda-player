using System;
using PandaPlayer.DiscAdder.Events;

namespace PandaPlayer.DiscAdder.ViewModels.SourceContent
{
	internal class SongTreeViewItem : EditableTreeViewItem
	{
		public event EventHandler<SongTitleChangingEventArgs> SongTitleChanging;

		public event EventHandler<SongTitleChangedEventArgs> SongTitleChanged;

		private string title;

		public string Title
		{
			get => title;
			set
			{
				string prevValue = title;

				// Exception, thrown during file renaming, won't blow up,
				// because exceptions thrown from binding properties are treated as validation failures.
				// http://stackoverflow.com/questions/12658220/exceptions-thrown-during-a-set-operation-in-a-property-are-not-being-caught
				// http://stackoverflow.com/questions/1488472/best-practices-throwing-exceptions-from-properties
				// It's not a big problem because song title will not be updated and still will be marked as incorrect.
				SongTitleChanging?.Invoke(this, new SongTitleChangingEventArgs(prevValue, value));
				Set(ref title, value);
				SongTitleChanged?.Invoke(this, new SongTitleChangedEventArgs(prevValue, value));
			}
		}

		private bool contentIsIncorrect;

		public bool ContentIsIncorrect
		{
			get => contentIsIncorrect;
			set => Set(ref contentIsIncorrect, value);
		}

		public SongTreeViewItem(SongContent song)
		{
			if (song == null)
			{
				throw new ArgumentNullException(nameof(song));
			}

			Title = song.Title;
		}
	}
}
