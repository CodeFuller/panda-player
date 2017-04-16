using CF.Library.Core.Bootstrap;
using CF.MusicLibrary.AlbumAdviser;
using Microsoft.Practices.Unity;
using NUnit.Framework;

namespace CF.MusicLibrary.IntegrationTests.CF.MusicLibrary.AlbumAdviser
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
		public void RegisterDependencies_RegistersAllDependenciesForApplicationLogic()
		{
			//	Arrange

			IUnityContainer diContainer = new UnityContainer();
			var target = new TestBootstrapper();

			//	Act

			target.InvokeRegisterDependencies(diContainer);

			//	Assert

			Assert.DoesNotThrow(() => diContainer.Resolve<IApplicationLogic>());
		}
	}
}
