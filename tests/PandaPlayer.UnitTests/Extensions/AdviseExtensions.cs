using PandaPlayer.Adviser.Grouping;
using PandaPlayer.Core.Models;

namespace PandaPlayer.UnitTests.Extensions
{
	internal static class AdviseExtensions
	{
		public static AdviseGroupContent ToAdviseGroup(this AdviseSetContent adviseSetContent)
		{
			var adviseGroup = new AdviseGroupContent(adviseSetContent.Id, isFavorite: false);
			foreach (var disc in adviseSetContent.Discs)
			{
				adviseGroup.AddDisc(disc);
			}

			return adviseGroup;
		}

		public static AdviseSetContent ToAdviseSet(this DiscModel disc)
		{
			var adviseSet = new AdviseSetContent($"Disc: {disc.Id}");
			adviseSet.AddDisc(disc);

			return adviseSet;
		}
	}
}
