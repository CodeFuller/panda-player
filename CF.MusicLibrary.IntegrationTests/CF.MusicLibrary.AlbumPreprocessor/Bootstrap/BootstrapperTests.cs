using CF.Library.Core.Configuration;
using CF.MusicLibrary.AlbumPreprocessor;
using CF.MusicLibrary.AlbumPreprocessor.ViewModels;
using Microsoft.Practices.Unity;
using NUnit.Framework;

namespace CF.MusicLibrary.IntegrationTests.CF.MusicLibrary.AlbumPreprocessor.Bootstrap
{
	internal class TestBootstrapper : Bootstrapper
	{
		public T Resolve<T>()
		{
			return DIContainer.Resolve<T>();
		}
	}

	[TestFixture]
	public class BootstrapperTests
	{
		[TearDown]
		public void TearDown()
		{
			AppSettings.ResetSettingsProvider();
		}

		[Test]
		public void Run_RegistersAllDependenciesForRootViewModel()
		{
			//	Arrange

			var target = new TestBootstrapper();

			//	Act

			target.Run();

			//	Assert

			Assert.DoesNotThrow(() => target.Resolve<MainWindowModel>());
		}

		[Test]
		public void Run_RegistersAllDependenciesForAddToLibraryViewModel()
		{
			//	Arrange

			var target = new TestBootstrapper();

			//	Act

			target.Run();

			//	Assert

			Assert.DoesNotThrow(() => target.Resolve<AddToLibraryViewModel>());
		}
	}
}
