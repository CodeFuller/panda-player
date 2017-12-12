using CF.Library.Core.Configuration;
using CF.Library.Core.Logging;
using CF.MusicLibrary.PandaPlayer;
using CF.MusicLibrary.PandaPlayer.ViewModels.Interfaces;
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
		}

		[Test]
		public void RegisterDependencies_RegistersSameInstanceForILoggerViewModelAndIMessageLoggerInterfaces()
		{
			//	Arrange

			var target = new BootstrapperHelper();

			//	Act

			target.Run();

			//	Assert

			var loggerViewModel = target.Container.Resolve<ILoggerViewModel>();
			var messageLogger = target.Container.Resolve<IMessageLogger>();
			Assert.AreSame(loggerViewModel, messageLogger);
		}
	}
}
