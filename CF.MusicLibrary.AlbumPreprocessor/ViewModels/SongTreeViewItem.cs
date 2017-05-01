using System;
using System.IO;

namespace CF.MusicLibrary.AlbumPreprocessor.ViewModels
{
	public class SongTreeViewItem : EditableTreeViewItem
	{
		private string AlbumPath { get; set; }

		private readonly AlbumTreeViewItem parentAlbum;

		public string SongFileName => GetSongFileName(Title);

		private string title;
		public string Title
		{
			get { return title; }
			set
			{
				if (title != null)
				{
					//	Exception, thrown at this point, won't blow up,
					//	because exceptions thrown from binding properties are treated as validation failures.
					//	http://stackoverflow.com/questions/12658220/exceptions-thrown-during-a-set-operation-in-a-property-are-not-being-caught
					//	http://stackoverflow.com/questions/1488472/best-practices-throwing-exceptions-from-properties
					//	It's not a big problem because song title will not be updated and still will be marked as incorrect.
					File.Move(GetSongFileName(title), GetSongFileName(value));
					parentAlbum.ReloadContent();
				}

				Set(ref title, value);
			}
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

		public SongTreeViewItem(AlbumTreeViewItem parentAlbum, SongContent song)
		{
			if (parentAlbum == null)
			{
				throw new ArgumentNullException(nameof(parentAlbum));
			}
			if (song == null)
			{
				throw new ArgumentNullException(nameof(song));
			}

			Title = song.Title;
			AlbumPath = parentAlbum.AlbumDirectory;

			this.parentAlbum = parentAlbum;
		}

		private string GetSongFileName(string songTitle)
		{
			return Path.Combine(AlbumPath, songTitle);
		}
	}
}
