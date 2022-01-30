using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using PandaPlayer.Core.Comparers;
using PandaPlayer.Core.Extensions;

namespace PandaPlayer.Core.Models
{
	public class DiscModel : BasicModel, INotifyPropertyChanged
	{
		private string title;
		private string treeTitle;
		private string albumTitle;

		public FolderModel Folder { get; internal set; }

		private AdviseGroupModel adviseGroup;

		public AdviseGroupModel AdviseGroup
		{
			get => adviseGroup;
			set => SetField(PropertyChanged, ref adviseGroup, value);
		}

		public AdviseSetInfo AdviseSetInfo { get; set; }

		public int? Year { get; set; }

		public string Title
		{
			get => title;
			set => SetField(PropertyChanged, ref title, value);
		}

		public string TreeTitle
		{
			get => treeTitle;
			set => SetField(PropertyChanged, ref treeTitle, value);
		}

		public string AlbumTitle
		{
			get => albumTitle;
			set => SetField(PropertyChanged, ref albumTitle, value);
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

		private readonly List<SongModel> allSongs = new();

		public IReadOnlyCollection<SongModel> AllSongs
		{
			get => allSongs;
			private init => allSongs = new List<SongModel>(value);
		}

		public IEnumerable<SongModel> AllSongsSorted => AllSongs
			.OrderBy(s => s.TrackNumber == null)
			.ThenBy(s => s.TrackNumber)
			.ThenBy(s => s.Artist == null)
			.ThenBy(s => s.Artist?.Name, StringComparer.InvariantCultureIgnoreCase)
			.ThenBy(s => s.TreeTitle, StringComparer.InvariantCultureIgnoreCase)
			.ThenBy(s => s.Id.Value);

		public IEnumerable<SongModel> ActiveSongs => AllSongsSorted.Where(song => !song.IsDeleted);

		private readonly List<DiscImageModel> images = new();

		public IReadOnlyCollection<DiscImageModel> Images
		{
			get => images;
			private init => images = new List<DiscImageModel>(value);
		}

		public DiscImageModel CoverImage => Images.SingleOrDefault(image => image.ImageType == DiscImageType.Cover);

		public bool IsDeleted => AllSongs.All(song => song.IsDeleted);

		public event PropertyChangedEventHandler PropertyChanged;

		public void AddSong(SongModel newSong)
		{
			allSongs.Add(newSong);
			newSong.Disc = this;
		}

		public void AddImage(DiscImageModel image)
		{
			if (image.ImageType == DiscImageType.Cover && CoverImage != null)
			{
				throw new InvalidOperationException("Disc can have only one cover image");
			}

			images.Add(image);
			image.Disc = this;
		}

		public void DeleteImage(DiscImageModel image)
		{
			var removed = images.RemoveAll(x => x.Id == image.Id);

			if (removed != 1)
			{
				throw new InvalidOperationException($"Can not delete an image from disc (Matched: {removed})");
			}
		}

		public DiscModel CloneShallow()
		{
			return new()
			{
				Id = Id,
				Folder = Folder,
				AdviseGroup = AdviseGroup,
				AdviseSetInfo = AdviseSetInfo,
				Year = Year,
				Title = Title,
				TreeTitle = TreeTitle,
				AlbumTitle = AlbumTitle,
				AllSongs = AllSongs,
				Images = Images,
			};
		}
	}
}
