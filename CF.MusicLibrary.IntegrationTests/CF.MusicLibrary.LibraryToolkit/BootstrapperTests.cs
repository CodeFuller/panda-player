using CF.Library.Core.Configuration;
using CF.MusicLibrary.LibraryToolkit;
using NUnit.Framework;

namespace CF.MusicLibrary.IntegrationTests.CF.MusicLibrary.LibraryToolkit
{
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

			var target = new Bootstrapper();

			//	Act & Assert

			Assert.DoesNotThrow(() => target.Run());
		}
	}
}
