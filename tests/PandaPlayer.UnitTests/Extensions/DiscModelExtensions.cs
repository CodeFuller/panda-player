using PandaPlayer.Core.Models;

namespace PandaPlayer.UnitTests.Extensions
{
	internal static class DiscModelExtensions
	{
		public static DiscModel AddToFolder(this DiscModel disc, FolderModel folder)
		{
			folder.AddDiscs(disc);
			return disc;
		}
	}
}
