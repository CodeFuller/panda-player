using PandaPlayer.Core.Models;

namespace PandaPlayer.Services.IntegrationTests.Data
{
	public partial class ReferenceData
	{
		public static ItemId FolderAdviseGroupId => new("1");

		public static ItemId DiscAdviseGroupId => new("2");

		public static ItemId NextAdviseGroupId => new("3");

		public AdviseGroupModel FolderAdviseGroup { get; } = new()
		{
			Id = FolderAdviseGroupId,
			Name = "Folder Advise Group",
			IsFavorite = false,
		};

		public AdviseGroupModel DiscAdviseGroup { get; } = new()
		{
			Id = DiscAdviseGroupId,
			Name = "Disc Advise Group",
			IsFavorite = true,
		};
	}
}
