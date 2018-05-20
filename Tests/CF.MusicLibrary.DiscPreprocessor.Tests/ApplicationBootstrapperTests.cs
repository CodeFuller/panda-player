using System;
using System.Collections.Generic;
using CF.Library.Core.Interfaces;
using CF.MusicLibrary.Common.Images;
using CF.MusicLibrary.Library;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using NUnit.Framework;

namespace CF.MusicLibrary.DiscPreprocessor.Tests
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
				{ "workshopStoragePath", @"c:\temp" },
				{ "fileSystemStorage:root", @"c:\temp" },
				{ "database:dataSource", @"Some DataSource" },
			};
			var target = new ApplicationBootstrapperHelper(settingValues);

			// Act & Assert

			Assert.DoesNotThrow(() => target.Bootstrap(Array.Empty<string>()));

			var imageFileFactory = target.ResolveDependency<IObjectFactory<IImageFile>>();
			Assert.DoesNotThrow(() => imageFileFactory.CreateInstance());
		}

		[Test]
		public void RegisterDependencies_BindsFileSystemStorageSettingsCorrectly()
		{
			// Arrange

			var settingValues = new Dictionary<string, string>
			{
				{ "dataStoragePath", @"c:\temp" },
				{ "workshopStoragePath", @"c:\temp" },
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
		public void RegisterDependencies_BindsDiscPreprocessorSettingsCorrectly()
		{
			// Arrange

			var settingValues = new Dictionary<string, string>
			{
				{ "dataStoragePath", @"Some DataStoragePath" },
				{ "workshopStoragePath", @"Some WorkshopStoragePath" },
				{ "fileSystemStorage:root", @"c:\temp" },
				{ "deleteSourceContentAfterAdding", @"True" },
				{ "database:dataSource", @"Some DataSource" },
			};
			var target = new ApplicationBootstrapperHelper(settingValues);

			// Act

			target.Bootstrap(Array.Empty<string>());

			// Assert

			var options = target.ResolveDependency<IOptions<DiscPreprocessorSettings>>();
			var settings = options.Value;

			Assert.AreEqual(@"Some DataStoragePath", settings.DataStoragePath);
			Assert.AreEqual(@"Some WorkshopStoragePath", settings.WorkshopStoragePath);
			Assert.IsTrue(settings.DeleteSourceContentAfterAdding);
		}
	}
}
