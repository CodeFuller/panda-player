using System;
using System.Threading.Tasks;
using CF.MusicLibrary.Core.Interfaces;
using CF.MusicLibrary.Core.Objects;
using CF.MusicLibrary.LibraryChecker.Registrators;
using static CF.Library.Core.Application;

namespace CF.MusicLibrary.LibraryChecker.Checkers
{
	class StorageConsistencyChecker : IStorageConsistencyChecker
	{
		private readonly IMusicLibrary musicLibrary;

		private readonly ILibraryInconsistencyRegistrator inconsistencyRegistrator;

		public StorageConsistencyChecker(IMusicLibrary musicLibrary, ILibraryInconsistencyRegistrator inconsistencyRegistrator)
		{
			if (musicLibrary == null)
			{
				throw new ArgumentNullException(nameof(musicLibrary));
			}
			if (inconsistencyRegistrator == null)
			{
				throw new ArgumentNullException(nameof(inconsistencyRegistrator));
			}

			this.musicLibrary = musicLibrary;
			this.inconsistencyRegistrator = inconsistencyRegistrator;
		}

		public async Task CheckStorage(DiscLibrary library, bool fixIssues)
		{
			Logger.WriteInfo("Checking library storage ...");
			await musicLibrary.CheckStorage(library, inconsistencyRegistrator, fixIssues);
		}

		public async Task CheckStorageChecksums(DiscLibrary library, bool fixIssues)
		{
			Logger.WriteInfo("Checking storage checksums...");
			await musicLibrary.CheckStorageChecksums(library, inconsistencyRegistrator, fixIssues);
		}
	}
}
