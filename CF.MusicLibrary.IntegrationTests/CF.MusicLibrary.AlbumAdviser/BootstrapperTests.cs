using CF.Library.Core.Bootstrap;
using CF.Library.Core.Configuration;
using CF.MusicLibrary.AlbumAdviser;
using Microsoft.Practices.Unity;
using NUnit.Framework;

namespace CF.MusicLibrary.IntegrationTests.CF.MusicLibrary.AlbumAdviser
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
		public void RegisterDependencies_RegistersAllDependenciesForApplicationLogic()
		{
			//	Arrange

			var target = new TestBootstrapper();

			//	Act

			target.Run();

			//	Assert

			Assert.DoesNotThrow(() => target.Resolve<IApplicationLogic>());
		}
	}
}
