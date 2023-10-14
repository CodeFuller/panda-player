using PandaPlayer.Core.Models;

namespace PandaPlayer.UnitTests.Helpers
{
	internal static class FolderModelExtensions
	{
		public static void AddSubfolders(this FolderModel folder, params FolderModel[] subfolders)
		{
			foreach (var subfolder in subfolders)
			{
				folder.AddSubfolder(subfolder);
			}
		}

		public static void AddDiscs(this FolderModel folder, params DiscModel[] discs)
		{
			foreach (var disc in discs)
			{
				folder.AddDisc(disc);
			}
		}
	}
}
