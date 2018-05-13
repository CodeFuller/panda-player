using System.Collections.Generic;
using CF.MusicLibrary.Dal;
using CF.MusicLibrary.LastFM;
using CF.MusicLibrary.Library;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using NUnit.Framework;

namespace CF.MusicLibrary.PandaPlayer.Tests
{
	[TestFixture]
	public class ApplicationBootstrapperTests
	{
		private class ApplicationBootstrapperHelper : ApplicationBootstrapper
		{
			private readonly Dictionary<string, string> settingValues;

			public ApplicationBootstrapperHelper(Dictionary<string, string> settingValues)
			{
				this.settingValues = settingValues;
			}

			public T ResolveDependency<T>()
			{
				return Resolve<T>();
			}

			protected override void BootstrapConfiguration(IConfigurationBuilder configurationBuilder, string[] commandLineArgs)
			{
				configurationBuilder.AddInMemoryCollection(settingValues);
			}
		}

		[Test]
		public void RegisterDependencies_RegistersAllDependenciesForApplicationLogic()
		{
			// Arrange

			var settingValues = new Dictionary<string, string>
			{
				{ "dataStoragePath", @"c:\temp" },
				{ "fileSystemStorage:root", @"c:\temp" },
			};
			var target = new ApplicationBootstrapperHelper(settingValues);

			// Act & Assert

			Assert.DoesNotThrow(() => target.Bootstrap(new string[0]));
		}

		[Test]
		public void RegisterDependencies_BindsSqLiteConnectionSettingsCorrectly()
		{
			// Arrange

			var settingValues = new Dictionary<string, string>
			{
				{ "dataStoragePath", @"c:\temp" },
				{ "fileSystemStorage:root", @"c:\temp" },
				{ "database:dataSource", @"Some DataSource" },
			};
			var target = new ApplicationBootstrapperHelper(settingValues);

			// Act

			target.Bootstrap(new string[0]);

			// Assert

			var options = target.ResolveDependency<IOptions<SqLiteConnectionSettings>>();
			var settings = options.Value;

			Assert.AreEqual(@"Some DataSource", settings.DataSource);
		}

		[Test]
		public void RegisterDependencies_BindsFileSystemStorageSettingsCorrectly()
		{
			// Arrange

			var settingValues = new Dictionary<string, string>
			{
				{ "dataStoragePath", @"c:\temp" },
				{ "fileSystemStorage:root", @"Some FileSystemStorage Root" },
			};
			var target = new ApplicationBootstrapperHelper(settingValues);

			// Act

			target.Bootstrap(new string[0]);

			// Assert

			var options = target.ResolveDependency<IOptions<FileSystemStorageSettings>>();
			var settings = options.Value;

			Assert.AreEqual(@"Some FileSystemStorage Root", settings.Root);
		}

		[Test]
		public void RegisterDependencies_BindsLastFmClientSettingsCorrectly()
		{
			// Arrange

			var settingValues = new Dictionary<string, string>
			{
				{ "dataStoragePath", @"c:\temp" },
				{ "fileSystemStorage:root", @"c:\temp" },
				{ "lastFmClient:apiKey", @"Some API Key" },
				{ "lastFmClient:sharedSecret", @"Some Shared Secret" },
				{ "lastFmClient:sessionKey", @"Some Session Key" },
			};
			var target = new ApplicationBootstrapperHelper(settingValues);

			// Act

			target.Bootstrap(new string[0]);

			// Assert

			var options = target.ResolveDependency<IOptions<LastFmClientSettings>>();
			var settings = options.Value;

			Assert.AreEqual(@"Some API Key", settings.ApiKey);
			Assert.AreEqual(@"Some Shared Secret", settings.SharedSecret);
			Assert.AreEqual(@"Some Session Key", settings.SessionKey);
		}

		[Test]
		public void RegisterDependencies_BindsPandaPlayerSettingsCorrectly()
		{
			// Arrange

			var settingValues = new Dictionary<string, string>
			{
				{ "dataStoragePath", @"c:\temp" },
				{ "fileSystemStorage:root", @"c:\temp" },
				{ "discCoverImageLookupPages:0", @"http://www.page1.com/" },
				{ "discCoverImageLookupPages:1", @"http://www.page2.com/" },
			};
			var target = new ApplicationBootstrapperHelper(settingValues);

			// Act

			target.Bootstrap(new string[0]);

			// Assert

			var options = target.ResolveDependency<IOptions<PandaPlayerSettings>>();
			var settings = options.Value;

			CollectionAssert.AreEqual(new[] { @"http://www.page1.com/", @"http://www.page2.com/" }, settings.DiscCoverImageLookupPages);
		}
	}
}
