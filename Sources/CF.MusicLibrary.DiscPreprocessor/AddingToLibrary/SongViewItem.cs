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

		public string ArtistName
		{
			get => Song.Artist?.Name;
			set
			{
				if (Song.Artist == null)
				{
					Song.Artist = new Artist
					{
						Name = value
					};
					RaisePropertyChanged();
				}
				else if (Song.Artist.Id == 0)
				{
					Song.Artist.Name = value;
					RaisePropertyChanged();
				}
				else
				{
					// Changing artist name for one song will affect all other songs of this artist (because Artist object is shared).
					// Moreover, existing Artist object in the database will also be updated.
					// This is probably not a desired behavior.
					throw new InvalidOperationException("Name could not be changed for the existing artist");
				}
			}
		}

		public string AlbumTitle => Song.Disc.AlbumTitle;

		public short? Year
		{
			get => Song.Year;
			set
			{
				Song.Year = value;
				RaisePropertyChanged();
			}
		}

		public Genre Genre
		{
			get => Song.Genre;
			set
			{
				Song.Genre = value;
				RaisePropertyChanged();
			}
		}

		public short? Track
		{
			get => Song.TrackNumber;
			set
			{
				Song.TrackNumber = value;
				RaisePropertyChanged();
			}
		}

		public string Title
		{
			get => Song.Title;
			set
			{
				Song.Title = value;
				RaisePropertyChanged();
			}
		}

		public SongViewItem(AddedSong addedSong)
		{
			AddedSong = addedSong;
		}
	}
}
