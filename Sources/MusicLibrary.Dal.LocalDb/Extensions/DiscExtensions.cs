using System.Collections.Generic;
using System.Linq;
using MusicLibrary.Core;
using MusicLibrary.Core.Objects;
using MusicLibrary.Dal.LocalDb.Interfaces;
using MusicLibrary.Logic.Models;

namespace MusicLibrary.Dal.LocalDb.Extensions
{
	internal static class DiscExtensions
	{
		public static DiscModel ToModel(this Disc disc, IDataStorage dataStorage)
		{
			var discModel = new DiscModel
			{
				Id = disc.Id.ToItemId(),
				Year = disc.Year,
				Title = disc.Title,
				TreeTitle = new ItemUriParts(disc.Uri).Last(),
				AlbumTitle = disc.AlbumTitle,
				CoverImageUri = disc.CoverImage != null ? dataStorage.TranslateInternalUri(disc.CoverImage.Uri) : null,
			};

			discModel.Songs = new List<SongModel>(disc.Songs.Select(s => s.ToModel(discModel)));

			return discModel;
		}
	}
}
