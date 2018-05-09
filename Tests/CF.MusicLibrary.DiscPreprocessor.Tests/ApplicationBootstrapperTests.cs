using System.Collections.Generic;
using CF.Library.Core.Interfaces;
using CF.MusicLibrary.Common.Images;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace CF.MusicLibrary.DiscPreprocessor.Tests
{
	[TestFixture]
	public class ApplicationBootstrapperTests
	{
		private class ApplicationBootstrapperHelper : ApplicationBootstrapper
		{
			public T ResolveDependency<T>()
			{
				return Resolve<T>();
			}

			protected override void BootstrapConfiguration(IConfigurationBuilder configurationBuilder, string[] commandLineArgs)
			{
				configurationBuilder.AddInMemoryCollection(new Dictionary<string, string>
				{
					{ "dataStoragePath", @"c:\temp" },
					{ "workshopStoragePath", @"c:\temp" },
					{ "fileSystemStorage:root", @"c:\temp" },
				});
			}
		}

		[Test]
		public void RegisterDependencies_RegistersAllDependenciesForApplicationLogic()
		{
			//	Arrange

			var target = new ApplicationBootstrapperHelper();

			//	Act & Assert

			Assert.DoesNotThrow(() => target.Bootstrap(new string[0]));

			var imageFileFactory = target.ResolveDependency<IObjectFactory<IImageFile>>();
			Assert.DoesNotThrow(() => imageFileFactory.CreateInstance());
		}
	}
}
