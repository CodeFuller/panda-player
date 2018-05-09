using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace CF.MusicLibrary.PandaPlayer.Tests
{
	[TestFixture]
	public class ApplicationBootstrapperTests
	{
		private class ApplicationBootstrapperHelper : ApplicationBootstrapper
		{
			protected override void BootstrapConfiguration(IConfigurationBuilder configurationBuilder, string[] commandLineArgs)
			{
				configurationBuilder.AddInMemoryCollection(new Dictionary<string, string>
				{
					{ "dataStoragePath", @"c:\temp" },
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
		}
	}
}
