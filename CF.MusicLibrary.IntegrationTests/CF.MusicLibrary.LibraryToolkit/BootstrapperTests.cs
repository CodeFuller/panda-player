using CF.MusicLibrary.LibraryToolkit;
using NUnit.Framework;

namespace CF.MusicLibrary.IntegrationTests.CF.MusicLibrary.LibraryToolkit
{
	[TestFixture]
	public class BootstrapperTests
	{
		[Test]
		public void RegisterDependencies_RegistersAllDependenciesForApplicationLogic()
		{
			//	Arrange

			var target = new ApplicationBootstrapper();

			//	Act & Assert

			Assert.DoesNotThrow(() => target.Bootstrap(new string[0]));
		}
	}
}
