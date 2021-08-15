using PandaPlayer.Core.Models;

namespace PandaPlayer.Services.IntegrationTests.Data
{
	public partial class ReferenceData
	{
		public static ItemId AdviseGroup1Id => new("1");

		public static ItemId AdviseGroup2Id => new("2");

		public static ItemId NextAdviseGroupId => new("3");

		public AdviseGroupModel AdviseGroup1 { get; } = new()
		{
			Id = AdviseGroup1Id,
			Name = "Late Neuro Dubel",
		};

		public AdviseGroupModel AdviseGroup2 { get; } = new()
		{
			Id = AdviseGroup2Id,
			Name = "Empty Group",
		};
	}
}
