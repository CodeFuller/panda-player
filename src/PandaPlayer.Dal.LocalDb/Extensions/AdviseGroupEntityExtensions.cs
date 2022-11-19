using PandaPlayer.Core.Models;
using PandaPlayer.Dal.LocalDb.Entities;

namespace PandaPlayer.Dal.LocalDb.Extensions
{
	internal static class AdviseGroupEntityExtensions
	{
		public static AdviseGroupModel ToModel(this AdviseGroupEntity adviseGroup)
		{
			return new()
			{
				Id = adviseGroup.Id.ToItemId(),
				Name = adviseGroup.Name,
				IsFavorite = adviseGroup.IsFavorite,
			};
		}

		public static AdviseGroupEntity ToEntity(this AdviseGroupModel adviseGroup)
		{
			return new()
			{
				Id = adviseGroup.Id?.ToInt32() ?? default,
				Name = adviseGroup.Name,
				IsFavorite = adviseGroup.IsFavorite,
			};
		}
	}
}
