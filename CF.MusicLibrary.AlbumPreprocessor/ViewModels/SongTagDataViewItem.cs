using System;
using CF.MusicLibrary.AlbumPreprocessor.AddingToLibrary;
using GalaSoft.MvvmLight;

namespace CF.MusicLibrary.AlbumPreprocessor.ViewModels
{
	public class SongTagDataViewItem : ViewModelBase
	{
		private readonly TaggedSongData data;

		public TaggedSongData TagData => data;

		public string SourceFileName => data.SourceFileName;

		public Uri StorageUri => data.StorageUri;

		public string Artist
		{
			get { return data.Artist; }
			set { data.Artist = value; RaisePropertyChanged(); }
		}

		public string Album
		{
			get { return data.Album; }
			set { data.Album = value; RaisePropertyChanged(); }
		}

		public int? Year
		{
			get { return data.Year; }
			set { data.Year = value; RaisePropertyChanged(); }
		}

		public string Genre
		{
			get { return data.Genre; }
			set { data.Genre = value; RaisePropertyChanged(); }
		}

		public int? Track
		{
			get { return data.Track; }
			set { data.Track = value; RaisePropertyChanged(); }
		}

		public string Title
		{
			get { return data.Title; }
			set { data.Title = value; RaisePropertyChanged(); }
		}

		public SongTagDataViewItem(TaggedSongData songData)
		{
			data = songData;
		}
	}
}
