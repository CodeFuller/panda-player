using System.Collections.Generic;
using System.Linq;

namespace MusicLibrary.Core.Models
{
	public class DiscModel
	{
		public ItemId Id { get; set; }

		public ShallowFolderModel Folder { get; set; }

		public int? Year { get; set; }

		public string Title { get; set; }

		public string TreeTitle { get; set; }

		public string AlbumTitle { get; set; }

		public IReadOnlyCollection<SongModel> AllSongs { get; set; }

		public IEnumerable<SongModel> ActiveSongs => AllSongs.Where(song => !song.IsDeleted);

		public IReadOnlyCollection<DiscImageModel> Images { get; set; }

		public DiscImageModel CoverImage => Images.SingleOrDefault(image => image.ImageType == DiscImageType.Cover);

		public bool IsDeleted => AllSongs.All(song => song.IsDeleted);
	}
}
