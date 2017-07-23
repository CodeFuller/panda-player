using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using CF.MusicLibrary.AlbumPreprocessor.Events;
using GalaSoft.MvvmLight.Messaging;

namespace CF.MusicLibrary.AlbumPreprocessor.ViewModels.SourceContent
{
	public class AlbumTreeViewItem : EditableTreeViewItem
	{
		public ReadOnlyCollection<SongTreeViewItem> SongItems { get; }

		public IEnumerable<SongTreeViewItem> Songs => SongItems.Take(SongItems.Count - 1);

		public IEnumerable<string> SongFileNames => Songs.Select(s => GetSongFileName(s.Title));

		private string albumDirectory;
		public string AlbumDirectory
		{
			get { return albumDirectory; }
			set
			{
				//	Value has changed or just initialized?
				bool valueChanged = (albumDirectory != null);

				if (valueChanged)
				{
					//	Exception, thrown at this point, won't blow up,
					//	because exceptions thrown from binding properties are treated as validation failures.
					//	http://stackoverflow.com/questions/12658220/exceptions-thrown-during-a-set-operation-in-a-property-are-not-being-caught
					//	http://stackoverflow.com/questions/1488472/best-practices-throwing-exceptions-from-properties
					//	It's not a big problem because directory title will not be updated and still will be marked as incorrect.
					Directory.Move(albumDirectory, value);
				}

				Set(ref albumDirectory, value);

				if (valueChanged)
				{
					OnAlbumContentChanged();
				}
			}
		}

		private static void OnAlbumContentChanged()
		{
			Messenger.Default.Send(new AlbumContentChangedEventArgs());
		}

		private bool contentIsIncorrect;
		public bool ContentIsIncorrect
		{
			get { return contentIsIncorrect; }
			set
			{
				Set(ref contentIsIncorrect, value);
			}
		}

		public AlbumTreeViewItem(AlbumContent album)
		{
			if (album == null)
			{
				throw new ArgumentNullException(nameof(album));
			}

			AlbumDirectory = album.AlbumDirectory;

			var songSeparator = new SongTreeViewItem(new SongContent(String.Empty));
			SongItems = new ReadOnlyCollection<SongTreeViewItem>(album.Songs.Select(CreateSongItem).Concat(Enumerable.Repeat(songSeparator, 1)).ToList());
		}

		private SongTreeViewItem CreateSongItem(SongContent songContent)
		{
			var songItem = new SongTreeViewItem(songContent);
			songItem.SongTitleChanging += SongTitle_Chaning;
			songItem.SongTitleChanged += SongTitle_Changed;

			return songItem;
		}

		private void SongTitle_Chaning(object sender, SongTitleChangingEventArgs e)
		{
			if (e.OldTitle != null)
			{
				File.Move(GetSongFileName(e.OldTitle), GetSongFileName(e.NewTitle));
			}
		}

		private void SongTitle_Changed(object sender, SongTitleChangedEventArgs e)
		{
			if (e.OldTitle != null)
			{
				OnAlbumContentChanged();
			}
		}

		private string GetSongFileName(string songTitle)
		{
			return Path.Combine(AlbumDirectory, songTitle);
		}
	}
}
