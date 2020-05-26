using System;
using MusicLibrary.Core.Models;
using MusicLibrary.Dal.LocalDb.Entities;
using MusicLibrary.Dal.LocalDb.Interfaces;
using DiscImageType = MusicLibrary.Core.Models.DiscImageType;

namespace MusicLibrary.Dal.LocalDb.Extensions
{
	internal static class DiscImageEntityExtensions
	{
		public static DiscImageModel ToModel(this DiscImageEntity discImage, DiscModel discModel, IContentUriProvider contentUriProvider)
		{
			var model = new DiscImageModel
			{
				Id = discImage.Id.ToItemId(),
				Disc = discModel,
				ImageType = ConvertImageType(discImage.ImageType),
				Size = discImage.FileSize,
			};

			model.ContentUri = contentUriProvider.GetDiscImageUri(model);

			return model;
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
