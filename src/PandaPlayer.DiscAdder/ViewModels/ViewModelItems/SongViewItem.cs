using System;
using GalaSoft.MvvmLight;
using PandaPlayer.Core.Models;
using PandaPlayer.DiscAdder.AddedContent;

namespace PandaPlayer.DiscAdder.ViewModels.ViewModelItems
{
	internal class SongViewItem : ViewModelBase
	{
		public AddedSong AddedSong { get; }

		private SongModel Song => AddedSong.Song;

		public string SourceFilePath => AddedSong.SourceFileName;

		public string ArtistName
		{
			get => Song.Artist?.Name;
			set
			{
				if (Song.Artist == null)
				{
					Song.Artist = new ArtistModel
					{
						Name = value,
					};

					RaisePropertyChanged();
				}
				else if (Song.Artist.Id == null)
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

		public GenreModel Genre
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

		public string TreeTitle
		{
			get => Song.TreeTitle;
			set
			{
				Song.TreeTitle = value;
				RaisePropertyChanged();
			}
		}

		public SongViewItem(AddedSong addedSong)
		{
			AddedSong = addedSong;
		}
	}
}
