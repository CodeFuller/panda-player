using System.Collections.Generic;

namespace PandaPlayer.Services.IntegrationTests.Data
{
	public class TestSessionData
	{
		public int NumericProperty { get; init; }

		public string StringProperty { get; init; }

		public IReadOnlyCollection<string> CollectionProperty { get; init; }
	}
}
