using CF.Library.Core.Bootstrap;
using CF.Library.Core.Facades;
using CF.Library.Core.Logging;
using CF.Library.Unity;
using Microsoft.Practices.Unity;

namespace CF.MusicLibrary.LibraryToolkit
{
	internal class Bootstrapper : UnityBootstrapper<IApplicationLogic>
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:DisposeObjectsBeforeLosingScope", Justification = "LifetimeManager is disposed by DI Container.")]
		protected override void RegisterDependencies()
		{
			DIContainer.RegisterType<IMessageLogger, ConsoleLogger>(new ContainerControlledLifetimeManager(), new InjectionConstructor(true));
			DIContainer.RegisterType<IFileSystemFacade, FileSystemFacade>();
			DIContainer.RegisterType<IApplicationLogic, ApplicationLogic>();
		}
	}
}
