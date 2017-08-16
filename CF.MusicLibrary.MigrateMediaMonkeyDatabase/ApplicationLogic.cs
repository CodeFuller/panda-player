using System;
using CF.Library.Core.Bootstrap;
using CF.MusicLibrary.BL;
using CF.MusicLibrary.BL.Objects;
using CF.MusicLibrary.Dal;

namespace CF.MusicLibrary.MigrateMediaMonkeyDatabase
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Class is instantiated by DI Container.")]
	internal class ApplicationLogic : IApplicationLogic
	{
		private readonly IMusicLibraryRepository musicLibraryRepository;

		public ApplicationLogic(IMusicLibraryRepository musicLibraryRepository)
		{
			if (musicLibraryRepository == null)
			{
				throw new ArgumentNullException(nameof(musicLibraryRepository));
			}

			this.musicLibraryRepository = musicLibraryRepository;
		}

		public int Run(string[] args)
		{
			Console.WriteLine("Loading library...");
			DiscLibrary library = musicLibraryRepository.GetDiscLibraryAsync().Result;

			Console.WriteLine("Adding discs...");
			using (var ctx = new MusicLibraryEntities())
			{
				ctx.Configuration.AutoDetectChangesEnabled = false;

				foreach (var disc in library)
				{
					ctx.Discs.Add(disc);
				}

				Console.WriteLine("Saving changes...");
				ctx.SaveChanges();

				Console.WriteLine("Finished successfully!");
			};

			return 0;
		}
	}
}
