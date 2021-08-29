using PandaPlayer.Adviser.Grouping;

namespace PandaPlayer.UnitTests.Helpers
{
	internal static class AdviseExtensions
	{
		public static AdviseGroupContent ToAdviseGroup(this AdviseSetContent adviseSetContent)
		{
			var adviseGroup = new AdviseGroupContent(adviseSetContent.Id);
			foreach (var disc in adviseSetContent.Discs)
			{
				adviseGroup.AddDisc(disc);
			}

			return adviseGroup;
		}
	}
}
