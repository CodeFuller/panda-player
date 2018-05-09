using CF.Library.Bootstrap;
using CF.Library.Configuration;
using CF.Library.Core.Facades;
using CF.Library.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CF.MusicLibrary.LibraryToolkit
{
	internal class ApplicationBootstrapper : DiApplicationBootstrapper<IApplicationLogic>
	{
		protected override void RegisterServices(IServiceCollection services, IConfiguration configuration)
		{
			services.AddTransient<IFileSystemFacade, FileSystemFacade>();
			services.AddTransient<IApplicationLogic, ApplicationLogic>();

		}

		protected override void BootstrapConfiguration(IConfigurationBuilder configurationBuilder, string[] commandLineArgs)
		{
			// We do not load configuration from command line, so that it does not conflict with utility commands.
			configurationBuilder.LoadSettings("AppSettings.json", new string[0]);
		}

		protected override void BootstrapLogging(ILoggerFactory loggerFactory, IConfiguration configuration)
		{
			loggerFactory.LoadLoggingConfiguration(configuration);
		}
	}
}
