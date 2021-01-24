using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace MusicLibrary.PandaPlayer.Tests
{
	[TestFixture]
	public class PandaPlayerSettingsTests
	{
		[Test]
		public void DiscCoverImageLookupPages_IfSomeLookupPagesAreConfigured_IsCorrectlyLoadedByConfigurationBinder()
		{
			// Arrange

			var configurationBuilder = new ConfigurationBuilder();
			configurationBuilder.AddInMemoryCollection(new Dictionary<string, string>
			{
				{ "discCoverImageLookupPages:0", @"http://www.page1.com/" },
				{ "discCoverImageLookupPages:1", @"http://www.page2.com/" },
			});
			var configuration = configurationBuilder.Build();

			var target = new PandaPlayerSettings();

			// Act

			configuration.Bind(target);

			// Assert

			CollectionAssert.AreEqual(new[] { @"http://www.page1.com/", @"http://www.page2.com/" }, target.DiscCoverImageLookupPages);
		}

		[Test]
		public void DiscCoverImageLookupPages_IfNoLookupPagesAreConfigured_IsNotNull()
		{
			// Arrange

			var configurationBuilder = new ConfigurationBuilder();
			configurationBuilder.AddInMemoryCollection(new Dictionary<string, string>());
			var configuration = configurationBuilder.Build();

			var target = new PandaPlayerSettings();

			// Act

			configuration.Bind(target);

			// Assert

			Assert.IsNotNull(target.DiscCoverImageLookupPages);
			CollectionAssert.IsEmpty(target.DiscCoverImageLookupPages);
		}
	}
}
