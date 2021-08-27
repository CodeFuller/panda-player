using PandaPlayer.Adviser.Grouping;
using PandaPlayer.Core.Models;

namespace PandaPlayer.UnitTests.Extensions
{
	internal static class DiscModelExtensions
	{
		public static AdviseSetContent ToAdviseSet(this DiscModel disc)
		{
			var adviseSet = new AdviseSetContent($"Disc: {disc.Id}");
			adviseSet.AddDisc(disc);

			return adviseSet;
		}
	}
}
