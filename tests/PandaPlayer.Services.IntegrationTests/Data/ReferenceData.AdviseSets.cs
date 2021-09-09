using PandaPlayer.Core.Models;

namespace PandaPlayer.Services.IntegrationTests.Data
{
	public partial class ReferenceData
	{
		public static ItemId AdviseSet1Id => new("1");

		public static ItemId AdviseSet2Id => new("2");

		public static ItemId NextAdviseSetId => new("3");

		public AdviseSetModel AdviseSet1 { get; } = new()
		{
			Id = AdviseSet1Id,
			Name = "Some Advise Set",
		};

		public AdviseSetModel AdviseSet2 { get; } = new()
		{
			Id = AdviseSet2Id,
			Name = "Another Advise Set",
		};

		private void FillAdviseSets()
		{
			NormalDisc.AdviseSet = AdviseSet1;
			NormalDisc.AdviseSetOrder = 1;
		}
	}
}
