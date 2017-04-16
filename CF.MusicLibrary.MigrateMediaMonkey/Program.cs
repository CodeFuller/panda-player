using System;
using System.Configuration;
using System.Data.Common;
using CF.MusicLibrary.BL;
using CF.MusicLibrary.BL.Objects;
using CF.MusicLibrary.Dal;
using CF.MusicLibrary.Dal.MediaMonkey;
using Microsoft.Practices.Unity;
using static System.FormattableString;

namespace CF.MusicLibrary.MigrateMediaMonkey
{
	class Program
	{
		private static IUnityContainer DIContainer { get; } = new UnityContainer();

		static void Main()
		{
			try
			{
				RegisterDIContainer();

				Console.WriteLine("Loading library...");
				IMusicLibraryRepository mdbRepository = DIContainer.Resolve<IMusicLibraryRepository>("MediaMonkey");
				DiscLibrary library = mdbRepository.LoadLibrary();

				Console.WriteLine("Updating library...");
				IMusicLibraryRepository efRepository = DIContainer.Resolve<IMusicLibraryRepository>("EF");
				efRepository.Store(library);
			}
			catch (Exception e)
			{
				Console.WriteLine(Invariant($"Exception caught: {e}"));
			}
		}

		private static void RegisterDIContainer()
		{
			var mediaMonkeyConnectionString = ConfigurationManager.ConnectionStrings["MediaMonkeyDB"];

			DIContainer.RegisterType<DbProviderFactory>(new InjectionFactory(context =>
				DbProviderFactories.GetFactory(mediaMonkeyConnectionString.ProviderName)));
			DIContainer.RegisterType<ILibraryBuilder, LibraryBuilder>();
			DIContainer.RegisterType<IMusicLibraryRepository, MusicLibraryRepository>("MediaMonkey",
				new InjectionConstructor(typeof(DbProviderFactory), typeof(ILibraryBuilder), mediaMonkeyConnectionString.ConnectionString));

			DIContainer.RegisterType<IMusicLibraryRepository, EFMusicLibraryRepository>("EF");
		}
	}
}
