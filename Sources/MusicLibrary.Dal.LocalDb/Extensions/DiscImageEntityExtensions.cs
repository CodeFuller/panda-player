using System;
using MusicLibrary.Core.Models;
using MusicLibrary.Dal.LocalDb.Entities;
using MusicLibrary.Dal.LocalDb.Interfaces;
using DiscImageType = MusicLibrary.Core.Models.DiscImageType;

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

		private static DiscImageType ConvertImageType(Entities.DiscImageType imageType)
		{
			switch (imageType)
			{
				case Entities.DiscImageType.Cover:
					return DiscImageType.Cover;

				default:
					throw new InvalidOperationException($"Unexpected disc image type: {imageType}");
			}
		}
	}
}
