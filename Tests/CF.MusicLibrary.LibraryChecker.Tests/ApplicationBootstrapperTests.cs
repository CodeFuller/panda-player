using System;
using System.Collections.Generic;
using System.Linq;
using CF.MusicLibrary.LastFM;
using CF.MusicLibrary.Library;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using NUnit.Framework;

namespace CF.MusicLibrary.LibraryChecker.Tests
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
				{ "fileSystemStorage:root", @"c:\temp" },
				{ "database:dataSource", @"Some DataSource" },
			};
			var target = new ApplicationBootstrapperHelper(settingValues);

			// Act & Assert

			Assert.DoesNotThrow(() => target.Bootstrap(Array.Empty<string>()));
		}

		[Test]
		public void RegisterDependencies_BindsCheckingSettingsCorrectly()
		{
			// Arrange

			var settingValues = new Dictionary<string, string>
			{
				{ "dataStoragePath", @"c:\temp" },
				{ "fileSystemStorage:root", @"c:\temp" },
				{ "database:dataSource", @"Some DataSource" },
				{ "checkingSettings:lastFmUsername", @"Some Last FM Username" },
			};
			var target = new ApplicationBootstrapperHelper(settingValues);

			// Act

			target.Bootstrap(Array.Empty<string>());

			// Assert

			var options = target.ResolveDependency<IOptions<CheckingSettings>>();
			var settings = options.Value;

			Assert.AreEqual(@"Some Last FM Username", settings.LastFmUsername);
		}

		[Test]
		public void RegisterDependencies_BindsInconsistencyFilterSettingsCorrectly()
		{
			// Arrange

			var settingValues = new Dictionary<string, string>
			{
				{ "dataStoragePath", @"c:\temp" },
				{ "fileSystemStorage:root", @"c:\temp" },
				{ "database:dataSource", @"Some DataSource" },
				{ "checkingSettings:inconsistencyFilter:skipDifferentGenresForDiscs:0", @"^/SomeCategory/SomeArtist" },
				{ "checkingSettings:inconsistencyFilter:allowedLastFmArtistCorrections:0:original", @"\bOf\b" },
				{ "checkingSettings:inconsistencyFilter:allowedLastFmArtistCorrections:0:corrected", @"of" },
				{ "checkingSettings:inconsistencyFilter:allowedLastFmSongCorrections:0:artist", @"Lacuna Coil" },
				{ "checkingSettings:inconsistencyFilter:allowedLastFmSongCorrections:0:original", @"1:19" },
				{ "checkingSettings:inconsistencyFilter:allowedLastFmSongCorrections:0:corrected", @"1.19" },
				{ "checkingSettings:inconsistencyFilter:lastFmSongTitleCharacterCorrections:0:original", @"A" },
				{ "checkingSettings:inconsistencyFilter:lastFmSongTitleCharacterCorrections:0:corrected", @"B" },
			};
			var target = new ApplicationBootstrapperHelper(settingValues);

			// Act

			target.Bootstrap(Array.Empty<string>());

			// Assert

			var options = target.ResolveDependency<IOptions<CheckingSettings>>();
			var filterSettings = options.Value.InconsistencyFilter;

			Assert.AreEqual(1, filterSettings.SkipDifferentGenresForDiscs.Count);
			Assert.AreEqual(@"^/SomeCategory/SomeArtist", filterSettings.SkipDifferentGenresForDiscs.Single());

			Assert.AreEqual(1, filterSettings.AllowedLastFmArtistCorrections.Count);
			var artistCorrection = filterSettings.AllowedLastFmArtistCorrections.Single();
			Assert.AreEqual(@"\bOf\b", artistCorrection.Original);
			Assert.AreEqual(@"of", artistCorrection.Corrected);

			Assert.AreEqual(1, filterSettings.AllowedLastFmSongCorrections.Count);
			var songCorrection = filterSettings.AllowedLastFmSongCorrections.Single();
			Assert.AreEqual(@"Lacuna Coil", songCorrection.Artist);
			Assert.AreEqual(@"1:19", songCorrection.Original);
			Assert.AreEqual(@"1.19", songCorrection.Corrected);

			Assert.AreEqual(1, filterSettings.LastFmSongTitleCharacterCorrections.Count);
			var titleCharCorrection = filterSettings.LastFmSongTitleCharacterCorrections.Single();
			Assert.AreEqual('A', titleCharCorrection.Original);
			Assert.AreEqual('B', titleCharCorrection.Corrected);
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
	}
}
