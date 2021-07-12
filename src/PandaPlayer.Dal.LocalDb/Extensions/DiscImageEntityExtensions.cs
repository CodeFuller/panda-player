using System;
using PandaPlayer.Core.Models;
using PandaPlayer.Dal.LocalDb.Entities;
using PandaPlayer.Dal.LocalDb.Interfaces;
using DiscImageType = PandaPlayer.Core.Models.DiscImageType;

namespace PandaPlayer.Dal.LocalDb.Extensions
{
	internal static class DiscImageEntityExtensions
	{
		public static DiscImageModel ToModel(this DiscImageEntity discImage, DiscModel discModel, IContentUriProvider contentUriProvider)
		{
			var model = new DiscImageModel
			{
				Id = discImage.Id.ToItemId(),
				Disc = discModel,
				TreeTitle = discImage.TreeTitle,
				ImageType = ConvertImageType(discImage.ImageType),
				Size = discImage.FileSize,
				Checksum = (uint)discImage.Checksum,
			};

			model.ContentUri = contentUriProvider.GetDiscImageUri(model);

			return model;
		}

		public static DiscImageEntity ToEntity(this DiscImageModel image)
		{
			return new DiscImageEntity
			{
				Id = image.Id?.ToInt32() ?? default,
				DiscId = image.Disc.Id.ToInt32(),
				TreeTitle = image.TreeTitle,
				ImageType = ConvertImageType(image.ImageType),
				FileSize = image.Size,
				Checksum = (int)image.Checksum,
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

		private static Entities.DiscImageType ConvertImageType(DiscImageType imageType)
		{
			switch (imageType)
			{
				case DiscImageType.Cover:
					return Entities.DiscImageType.Cover;

				default:
					throw new InvalidOperationException($"Unexpected disc image type: {imageType}");
			}
		}
	}
}
