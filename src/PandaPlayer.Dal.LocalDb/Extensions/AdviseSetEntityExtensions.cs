using PandaPlayer.Core.Models;
using PandaPlayer.Dal.LocalDb.Entities;

namespace PandaPlayer.Dal.LocalDb.Extensions
{
	internal static class AdviseSetEntityExtensions
	{
		public static AdviseSetModel ToModel(this AdviseSetEntity adviseSet)
		{
			return new()
			{
				Id = adviseSet.Id.ToItemId(),
				Name = adviseSet.Name,
			};
		}

		public static AdviseSetEntity ToEntity(this AdviseSetModel adviseSet)
		{
			return new()
			{
				Id = adviseSet.Id?.ToInt32() ?? default,
				Name = adviseSet.Name,
			};
		}
	}
}
