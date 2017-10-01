using CF.Library.Core.Configuration;
using CF.Library.Core.Interfaces;
using CF.MusicLibrary.Common.DiscArt;
using CF.MusicLibrary.DiscPreprocessor;
using Microsoft.Practices.Unity;
using NSubstitute;
using NUnit.Framework;

namespace CF.MusicLibrary.IntegrationTests.CF.MusicLibrary.DiscPreprocessor
{
	[TestFixture]
	public class BootstrapperTests
	{
		private class BootstrapperHelper : Bootstrapper
		{
			public IUnityContainer Container => DIContainer;
		}

		[TearDown]
		public void TearDown()
		{
			AppSettings.ResetSettingsProvider();
		}

		[Test]
		public void RegisterDependencies_RegistersAllDependenciesForApplicationLogic()
		{
			//	Arrange

			AppSettings.SettingsProvider = Substitute.For<ISettingsProvider>();
			var target = new BootstrapperHelper();

			//	Act & Assert

			Assert.DoesNotThrow(() => target.Run());

			var discArtImageFileFactory = target.Container.Resolve<IObjectFactory<IDiscArtImageFile>>();
			Assert.DoesNotThrow(() => discArtImageFileFactory.CreateInstance());
		}
	}
}
