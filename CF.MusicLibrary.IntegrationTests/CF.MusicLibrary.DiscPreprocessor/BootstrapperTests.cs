using CF.Library.Core.Configuration;
using CF.MusicLibrary.DiscPreprocessor;
using NSubstitute;
using NUnit.Framework;

namespace CF.MusicLibrary.IntegrationTests.CF.MusicLibrary.DiscPreprocessor
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

			AppSettings.SettingsProvider = Substitute.For<ISettingsProvider>();
			var target = new Bootstrapper();

			//	Act & Assert

			Assert.DoesNotThrow(() => target.Run());
		}
	}
}
