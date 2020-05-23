using System;
using MusicLibrary.Dal.LocalDb.Entities;
using MusicLibrary.Dal.LocalDb.Interfaces;
using MusicLibrary.Logic.Models;

namespace MusicLibrary.Dal.LocalDb.Extensions
{
	internal static class DiscImageEntityExtensions
	{
		public static DiscImageModel ToModel(this DiscImageEntity discImage, IDataStorage dataStorage)
		{
			return new DiscImageModel
			{
				Id = discImage.Id.ToItemId(),
				ImageType = ConvertImageType(discImage.ImageType),
				Uri = dataStorage.TranslateInternalUri(discImage.Uri),
				Size = discImage.FileSize,
			};
		}

		private static Logic.Models.DiscImageType ConvertImageType(Entities.DiscImageType imageType)
		{
			switch (imageType)
			{
				case Entities.DiscImageType.Cover:
					return Logic.Models.DiscImageType.Cover;

				default:
					throw new InvalidOperationException($"Unexpected disc image type: {imageType}");
			}
		}
	}
}
