using System;
using System.Collections.Generic;
using CF.MusicLibrary.Dal;
using CF.MusicLibrary.LastFM;
using CF.MusicLibrary.Library;
using CF.MusicLibrary.PandaPlayer.Adviser.Grouping;
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
				{ "database:dataSource", @"Some DataSource" },
			};
			var target = new ApplicationBootstrapperHelper(settingValues);

			// Act & Assert

			Assert.DoesNotThrow(() => target.Bootstrap(Array.Empty<string>()));
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

			target.Bootstrap(Array.Empty<string>());

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
				{ "database:dataSource", @"Some DataSource" },
			};
			var target = new ApplicationBootstrapperHelper(settingValues);

			// Act

			target.Bootstrap(Array.Empty<string>());

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
				{ "database:dataSource", @"Some DataSource" },
				{ "lastFmClient:apiKey", @"Some API Key" },
				{ "lastFmClient:sharedSecret", @"Some Shared Secret" },
				{ "lastFmClient:sessionKey", @"Some Session Key" },
			};
			var target = new ApplicationBootstrapperHelper(settingValues);

			// Act

			target.Bootstrap(Array.Empty<string>());

			// Assert

			var options = target.ResolveDependency<IOptions<LastFmClientSettings>>();
			var settings = options.Value;

			Assert.AreEqual(@"Some API Key", settings.ApiKey);
			Assert.AreEqual(@"Some Shared Secret", settings.SharedSecret);
			Assert.AreEqual(@"Some Session Key", settings.SessionKey);
		}

		[Test]
		public void RegisterDependencies_BindsGroupingSettingsCorrectly()
		{
			// Arrange

			var settingValues = new Dictionary<string, string>
			{
				{ "dataStoragePath", @"c:\temp" },
				{ "fileSystemStorage:root", @"c:\temp" },
				{ "database:dataSource", @"Some DataSource" },
				{ "adviser:groupings:0:pattern", "Pattern 1" },
				{ "adviser:groupings:0:groupId", "GroupId 1" },
				{ "adviser:groupings:1:pattern", "Pattern 2" },
				{ "adviser:groupings:1:groupId", "GroupId 2" },
			};
			var target = new ApplicationBootstrapperHelper(settingValues);

			// Act

			target.Bootstrap(Array.Empty<string>());

			// Assert

			var options = target.ResolveDependency<IOptions<GroupingSettings>>();
			var settings = options.Value;

			Assert.AreEqual(2, settings.Count);
			Assert.AreEqual("Pattern 1", settings[0].Pattern);
			Assert.AreEqual("GroupId 1", settings[0].GroupId);
			Assert.AreEqual("Pattern 2", settings[1].Pattern);
			Assert.AreEqual("GroupId 2", settings[1].GroupId);
		}

		[Test]
		public void RegisterDependencies_BindsPandaPlayerSettingsCorrectly()
		{
			// Arrange

			var settingValues = new Dictionary<string, string>
			{
				{ "dataStoragePath", @"c:\temp" },
				{ "fileSystemStorage:root", @"c:\temp" },
				{ "database:dataSource", @"Some DataSource" },
				{ "discCoverImageLookupPages:0", @"http://www.page1.com/" },
				{ "discCoverImageLookupPages:1", @"http://www.page2.com/" },
			};
			var target = new ApplicationBootstrapperHelper(settingValues);

			// Act

			target.Bootstrap(Array.Empty<string>());

			// Assert

			var options = target.ResolveDependency<IOptions<PandaPlayerSettings>>();
			var settings = options.Value;

			CollectionAssert.AreEqual(new[] { @"http://www.page1.com/", @"http://www.page2.com/" }, settings.DiscCoverImageLookupPages);
		}
	}
}
