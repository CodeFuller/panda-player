using CF.Library.Core.Configuration;
using CF.Library.Core.Interfaces;
using CF.MusicLibrary.Common.Images;
using CF.MusicLibrary.DiscPreprocessor;
using NSubstitute;
using NUnit.Framework;
using Unity;

namespace CF.MusicLibrary.IntegrationTests.CF.MusicLibrary.DiscPreprocessor
{
	[TestFixture]
	public class BootstrapperTests
	{
		private class BootstrapperHelper : Bootstrapper
		{
			public IUnityContainer Container => DIContainer;

			protected override void OnDependenciesRegistering()
			{
				DIContainer.RegisterInstance(Substitute.For<ISettingsProvider>());
			}
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

			var target = new BootstrapperHelper();

			//	Act & Assert

			Assert.DoesNotThrow(() => target.Run());

			var imageFileFactory = target.Container.Resolve<IObjectFactory<IImageFile>>();
			Assert.DoesNotThrow(() => imageFileFactory.CreateInstance());
		}
	}
}
