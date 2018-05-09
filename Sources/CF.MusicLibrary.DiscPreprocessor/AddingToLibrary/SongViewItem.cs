using System;
using CF.MusicLibrary.Core.Objects;
using GalaSoft.MvvmLight;

namespace CF.MusicLibrary.DiscPreprocessor.AddingToLibrary
{
	public class SongViewItem : ViewModelBase
	{
		public AddedSong AddedSong { get; }

		private Song Song => AddedSong.Song;

		public Uri StorageUri => Song.Uri;

		//	See comment in XAML why Artist column is read-only
		public string ArtistName => Song.Artist?.Name;

		public string AlbumTitle => Song.Disc.AlbumTitle;

		public short? Year
		{
			get => Song.Year;
			set { Song.Year = value; RaisePropertyChanged(); }
		}

		public Genre Genre
		{
			get => Song.Genre;
			set { Song.Genre = value; RaisePropertyChanged(); }
		}

		public short? Track
		{
			get => Song.TrackNumber;
			set { Song.TrackNumber = value; RaisePropertyChanged(); }
		}

		public string Title
		{
			get => Song.Title;
			set { Song.Title = value; RaisePropertyChanged(); }
		}

		public SongViewItem(AddedSong addedSong)
		{
			AddedSong = addedSong;
		}
	}
}
