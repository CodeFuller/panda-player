using System;
using GalaSoft.MvvmLight;
using PandaPlayer.Core.Models;
using PandaPlayer.DiscAdder.MusicStorage;

namespace PandaPlayer.DiscAdder.ViewModels.ViewModelItems
{
	internal class SongViewItem : ViewModelBase
	{
		private readonly AddedSongInfo addedSongInfo;

		public DiscViewItem DiscItem { get; }

		public DiscModel ExistingDisc => DiscItem.ExistingDisc;

		public string DiscSourcePath => DiscItem.SourcePath;

		public string SourceFilePath => addedSongInfo.SourcePath;

		private string artistName;

		public string ArtistName
		{
			get => artistName;
			set => Set(ref artistName, value);
		}

		public string AlbumTitle => DiscItem.AlbumTitle;

		public GenreModel Genre => DiscItem.Genre;

		public short? Track
		{
			get => addedSongInfo.Track;
			set
			{
				addedSongInfo.Track = value;
				RaisePropertyChanged();
			}
		}

		public string Title
		{
			get => addedSongInfo.Title;
			set
			{
				addedSongInfo.Title = value;
				RaisePropertyChanged();
			}
		}

		public string TreeTitle
		{
			get => addedSongInfo.TreeTitle;
			set
			{
				addedSongInfo.TreeTitle = value;
				RaisePropertyChanged();
			}
		}

		public SongViewItem(DiscViewItem discItem, AddedSongInfo addedSongInfo)
		{
			DiscItem = discItem ?? throw new ArgumentNullException(nameof(discItem));
			this.addedSongInfo = addedSongInfo ?? throw new ArgumentNullException(nameof(addedSongInfo));

			ArtistName = discItem.Artist.ArtistName ?? addedSongInfo.Artist;
		}
	}
}
