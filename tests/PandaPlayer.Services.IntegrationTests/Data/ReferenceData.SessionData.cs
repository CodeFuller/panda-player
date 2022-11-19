namespace PandaPlayer.Services.IntegrationTests.Data
{
	public partial class ReferenceData
	{
		public static string ExistingSessionDataKey => "Existing Data Key";

		public static string NonExistingSessionDataKey => "New Data Key";

		public TestSessionData ExistingSessionData { get; } = new()
		{
			NumericProperty = 12345,
			StringProperty = "StringProperty From Database",
			CollectionProperty = new[]
			{
				"CollectionValue1 From Database",
				"CollectionValue2 From Database",
			},
		};
	}
}
