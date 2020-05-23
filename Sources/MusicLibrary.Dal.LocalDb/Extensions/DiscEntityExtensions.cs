using System.Collections.Generic;
using System.Linq;
using CF.Library.Core.Extensions;
using MusicLibrary.Core;
using MusicLibrary.Dal.LocalDb.Entities;
using MusicLibrary.Dal.LocalDb.Interfaces;
using MusicLibrary.Logic.Models;

namespace MusicLibrary.Dal.LocalDb.Extensions
{
	internal static class DiscEntityExtensions
	{
		public static DiscModel ToModel(this DiscEntity disc, IDataStorage dataStorage)
		{
			// TBD: Add year as disc column in database and remove this logic
			var discYear = disc.Songs
				.Where(s => s.DeleteDate != null)
				.Select(s => s.Year)
				.UniqueOrDefault();

			var coverImage = disc.Images
				.SingleOrDefault(im => im.ImageType == DiscImageType.Cover);

			var discModel = new DiscModel
			{
				Id = disc.Id.ToItemId(),
				Year = discYear,
				Title = disc.Title,
				TreeTitle = new ItemUriParts(disc.Uri).Last(),
				AlbumTitle = disc.AlbumTitle,
				CoverImageUri = coverImage != null ? dataStorage.TranslateInternalUri(coverImage.Uri) : null,
			};

			discModel.Songs = new List<SongModel>(disc.Songs.Select(s => s.ToModel(discModel, dataStorage)));

			return discModel;
		}
	}
}
