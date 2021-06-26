using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using MusicLibrary.Core.Comparers;
using MusicLibrary.Core.Extensions;

namespace MusicLibrary.Core.Models
{
	public class DiscModel : INotifyPropertyChanged
	{
		private string title;
		private string treeTitle;
		private string albumTitle;

		public ItemId Id { get; set; }

		public ShallowFolderModel Folder { get; set; }

		public int? Year { get; set; }

		public string Title
		{
			get => title;
			set => this.SetField(PropertyChanged, ref title, value);
		}

		public string TreeTitle
		{
			get => treeTitle;
			set => this.SetField(PropertyChanged, ref treeTitle, value);
		}

		public string AlbumTitle
		{
			get => albumTitle;
			set => this.SetField(PropertyChanged, ref albumTitle, value);
		}

		public ArtistModel SoloArtist
		{
			get
			{
				return (IsDeleted ? AllSongs : ActiveSongs)
					.Select(song => song.Artist)
					.UniqueOrDefault(new ArtistEqualityComparer());
			}
		}

		public IReadOnlyCollection<SongModel> AllSongs { get; set; }

		public IEnumerable<SongModel> ActiveSongs => AllSongs
			.Where(song => !song.IsDeleted)
			.OrderBy(s => s.TrackNumber == null)
			.ThenBy(s => s.TrackNumber)
			.ThenBy(s => s.Artist == null)
			.ThenBy(s => s.Artist?.Name, StringComparer.OrdinalIgnoreCase)
			.ThenBy(s => s.TreeTitle, StringComparer.OrdinalIgnoreCase)
			.ThenBy(s => s.Id.Value);

		private List<DiscImageModel> images = new();

		public IReadOnlyCollection<DiscImageModel> Images
		{
			get => images;
			set => images = new List<DiscImageModel>(value);
		}

		public DiscImageModel CoverImage => Images.SingleOrDefault(image => image.ImageType == DiscImageType.Cover);

		public bool IsDeleted => AllSongs.All(song => song.IsDeleted);

		public event PropertyChangedEventHandler PropertyChanged;

		public void AddImage(DiscImageModel image)
		{
			if (image.ImageType == DiscImageType.Cover && CoverImage != null)
			{
				throw new InvalidOperationException("Disc can have only one cover image");
			}

			images.Add(image);
		}

		public void DeleteImage(DiscImageModel image)
		{
			var removed = images.RemoveAll(x => x.Id == image.Id);

			if (removed != 1)
			{
				throw new InvalidOperationException($"Can not delete an image from disc (Matched: {removed})");
			}
		}
	}
}
