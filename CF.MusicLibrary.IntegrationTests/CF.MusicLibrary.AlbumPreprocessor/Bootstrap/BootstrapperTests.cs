using CF.MusicLibrary.AlbumPreprocessor;
using CF.MusicLibrary.AlbumPreprocessor.ViewModels;
using Microsoft.Practices.Unity;
using NUnit.Framework;

namespace CF.MusicLibrary.IntegrationTests.CF.MusicLibrary.AlbumPreprocessor.Bootstrap
{
	internal class TestBootstrapper : Bootstrapper
	{
		public void InvokeRegisterDependencies(IUnityContainer container)
		{
			RegisterDependencies(container);
		}
	}

	[TestFixture]
	public class BootstrapperTests
	{
		[Test]
		public void Run_RegistersAllDependenciesForRootViewModel()
		{
			//	Arrange

			IUnityContainer diContainer = new UnityContainer();
			var target = new TestBootstrapper();

			//	Act

			target.InvokeRegisterDependencies(diContainer);

			//	Assert

			Assert.DoesNotThrow(() => diContainer.Resolve<MainWindowModel>());
		}
	}
}
