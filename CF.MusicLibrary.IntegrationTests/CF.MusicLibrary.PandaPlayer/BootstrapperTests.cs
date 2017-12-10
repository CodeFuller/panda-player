using CF.Library.Core.Configuration;
using CF.MusicLibrary.PandaPlayer;
using NSubstitute;
using NUnit.Framework;
using Unity;

namespace CF.MusicLibrary.IntegrationTests.CF.MusicLibrary.PandaPlayer
{
	[TestFixture]
	public class BootstrapperTests
	{
		private class BootstrapperHelper : Bootstrapper
		{
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
		}
	}
}
